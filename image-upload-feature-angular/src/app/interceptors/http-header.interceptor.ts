import { HttpInterceptorFn, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Token } from '../models/auth.model';

export const httpHeaderInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  const skipContentType = req.headers.get('Skip-Content-Type') === 'true';
  let headers = req.headers;

  const isFormData = req.body instanceof FormData;
  const shouldSetJson = !skipContentType && !isFormData && req.method !== 'GET';

  if (shouldSetJson) {
    headers = headers.set('Content-Type', 'application/json').set('Accept', 'application/json');
  }
  if (skipContentType) {
    headers = headers.delete('Skip-Content-Type');
  }

  const excludeUrls = [
    { url: 'Authenticate/login', methods: ['POST'] },
    { url: 'Authenticate/verify', methods: ['PUT'] },
    { url: 'Authenticate/verify/resend-code', methods: ['PUT'] },
    { url: 'Authenticate/refresh-token', methods: ['POST'] },
    { url: 'User', methods: ['POST'] },
    { url: 'User/forgot-password', methods: ['POST'] },
    { url: 'User/reset-password', methods: ['POST'] },
  ];

  const isExcluded = excludeUrls.some(e => req.url.endsWith(e.url) && e.methods.includes(req.method));

  if (!isExcluded && token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  const cloned = req.clone({ headers });

  return next(cloned).pipe(
    catchError((originalError: HttpErrorResponse) => {
      const hasRefresh = !!authService.getRefreshToken?.();
      const isRefreshCall = req.url.endsWith('Authenticate/refresh-token');

      if (originalError.status === 401 && !isExcluded && !isRefreshCall && hasRefresh) {
        return authService.generateRefreshToken().pipe(
          switchMap((newTokens: Token) => {
            authService.saveTokens(newTokens);
            return next(
              req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newTokens.accessToken}`,
                },
              })
            );
          }),
          catchError(error => {
            authService.logout();
            return throwError(() => error);
          })
        );
      }

      return throwError(() => originalError);
    })
  );
};
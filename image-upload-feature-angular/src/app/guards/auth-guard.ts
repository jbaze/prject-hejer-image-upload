// src/app/guards/auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

canActivate(
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
): Observable<boolean> | Promise<boolean> | boolean {
  const email = localStorage.getItem('ImageStorageEmail');
  const password = localStorage.getItem('ImageStoragePass');

  if (email && password) {
    return this.authService.login({ email: email!, password: password! }).pipe(
      map((response) => {
        this.authService.currentUserSubject.next(response?.data?.user);
        this.authService.isAuthenticatedSubject.next(true);
        this.authService.saveTokens({accessToken: response.data.accessToken, refreshToken: response.data.refreshToken});
        this.authService.redirectUrl = state.url;

        return true;
      }),
      catchError(() => {
        this.authService.logout();
        this.router.navigate(['/auth/login']);
        return of(false);
      })
    );
  } else {
    this.router.navigate(['/auth/login']);
    return false;
  }
}
}

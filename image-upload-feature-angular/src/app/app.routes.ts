import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth-guard';
import { UsersComponent } from './components/users/users.component';

export const routes: Routes = [
    {
        path: '',
        component: UsersComponent,
        pathMatch: 'full',
        canActivate: [AuthGuard],
    },
    {
        path: 'users',
        component: UsersComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'auth',
        loadChildren: () => import('./auth/auth.routes').then(m => m.AUTH_ROUTES),
    },
    {
        path: '**',
        redirectTo: '/auth',
    },
];

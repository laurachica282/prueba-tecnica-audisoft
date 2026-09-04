import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import type { AppRole } from './auth.service';

export const authGuard: CanActivateFn = () => {
    const auth = inject(AuthService);

    if (auth.hasValidToken()) return true;

    auth.login();
    return false;
};

export const roleGuard = (allowed: AppRole[]): CanActivateFn => {
    return () => {
        const auth = inject(AuthService);
        const router = inject(Router);

        if (!auth.hasValidToken()) {
            auth.login();
            return false;
        }

        const user = auth.currentUser();
        const permitted = user?.roles.some((role) => allowed.includes(role)) ?? false;

        return permitted ? true : router.createUrlTree(['/forbidden']);
    };
};
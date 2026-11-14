import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { API_URL } from '../tokens/api-url.token';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const apiUrl = inject(API_URL);

    if (!req.url.startsWith(`${apiUrl}/`)) {
        return next(req);
    }

    const isAuthEndpoint = req.url.startsWith(`${apiUrl}/auth/`);
    const token = auth.token();
    const request =
        token && !isAuthEndpoint
            ? req.clone({ setHeaders : { Authorization: `Bearer ${token}` } })
            : req;
    
    return next(request).pipe(
        catchError((error: unknown) => {
            if (error instanceof HttpErrorResponse && error.status === 401 && !isAuthEndpoint) {
                auth.logout();
                void router.navigate(['/login']);
            }
            return throwError(() => error);
        })
    )
}
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/**
 * Dokleja JWT do żądań i centralnie obsługuje odpowiedzi auth:
 * 401 (wygasła sesja) → wylogowanie i przekierowanie na /login,
 * 402 (paywall) → przekierowanie na /account z banerem premium.
 * Endpointy /auth/ są pominięte — ich błędy obsługują formularze.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const token = auth.token();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse && !req.url.includes('/auth/')) {
        if (err.status === 401) {
          auth.logout();
          router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
        } else if (err.status === 402) {
          router.navigate(['/account'], { queryParams: { paywall: 1 } });
        }
      }
      return throwError(() => err);
    })
  );
};

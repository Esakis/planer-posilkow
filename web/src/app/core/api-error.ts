import { HttpErrorResponse } from '@angular/common/http';
import { TimeoutError } from 'rxjs';

/** Czy błąd oznacza brak połączenia z serwerem (a nie odpowiedź serwera z błędem). */
export function isNetworkError(err: unknown): boolean {
  return err instanceof TimeoutError ||
    (err instanceof HttpErrorResponse && err.status === 0);
}

/** Wyciąga komunikat błędu API ({ message }) albo zwraca fallback. */
export function apiErrorMessage(err: unknown, fallback: string): string {
  if (err instanceof TimeoutError) {
    return 'Serwer nie odpowiada. Spróbuj ponownie za chwilę.';
  }
  if (err instanceof HttpErrorResponse && typeof err.error?.message === 'string') {
    return err.error.message;
  }
  return fallback;
}

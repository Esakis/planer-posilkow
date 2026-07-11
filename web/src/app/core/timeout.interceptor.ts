import { HttpInterceptorFn } from '@angular/common/http';
import { timeout } from 'rxjs';

/** Po tylu ms przerywamy żądanie, żeby UI nie wisiał na zamrożonym API. */
const TIMEOUT_MS = 15_000;

export const timeoutInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(timeout(TIMEOUT_MS));

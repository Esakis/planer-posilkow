import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Account, LoginResponse, RegisterResponse } from './models';
import { readJson, removeKey, writeJson } from './storage';

const API = '/api';
const TOKEN_KEY = 'tt_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  /** JWT — trzymany w localStorage, żeby sesja przetrwała odświeżenie. */
  readonly token = signal<string | null>(readJson<string>(TOKEN_KEY));
  /** Stan konta z GET /auth/me — null, dopóki nie pobrany. */
  readonly account = signal<Account | null>(null);
  readonly isLoggedIn = computed(() => this.token() !== null);

  register(email: string, password: string): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${API}/auth/register`, { email, password });
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${API}/auth/login`, { email, password }).pipe(
      tap(res => {
        writeJson(TOKEN_KEY, res.token);
        this.token.set(res.token);
        this.account.set(res.account);
      })
    );
  }

  /** Aktywacja konta linkiem z maila — startuje 30-dniowy trial. */
  activate(token: string): Observable<Account> {
    return this.http.post<Account>(`${API}/auth/activate`, { token }).pipe(
      // aktywować można też z niezalogowanej przeglądarki — wtedy konta nie nadpisujemy
      tap(acc => { if (this.isLoggedIn()) this.account.set(acc); })
    );
  }

  refreshAccount(): Observable<Account> {
    return this.http.get<Account>(`${API}/auth/me`).pipe(tap(acc => this.account.set(acc)));
  }

  /** Sesja Stripe Checkout — zwraca URL do przekierowania na płatność. */
  checkout(): Observable<{ url: string }> {
    return this.http.post<{ url: string }>(`${API}/billing/checkout`, {});
  }

  /** Potwierdzenie płatności po powrocie z Checkout (?session_id=...). */
  confirmPayment(sessionId: string): Observable<Account> {
    return this.http.post<Account>(`${API}/billing/confirm`, { sessionId }).pipe(
      tap(acc => this.account.set(acc))
    );
  }

  logout(): void {
    removeKey(TOKEN_KEY);
    this.token.set(null);
    this.account.set(null);
  }
}

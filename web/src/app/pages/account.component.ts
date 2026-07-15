import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { apiErrorMessage } from '../core/api-error';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [DatePipe],
  template: `
    <div class="container stack" style="max-width:520px">
      <h1 style="font-size:22px">Twoje konto</h1>

      @if (paywall()) {
        <div class="notice warn">Ta funkcja wymaga aktywnego planu — szczegóły poniżej.</div>
      }
      @if (paymentCancelled()) {
        <div class="notice warn">Płatność została przerwana. Możesz spróbować ponownie.</div>
      }
      @if (paymentSuccess()) {
        <div class="notice">🎉 Płatność potwierdzona — masz plan Premium. Dziękujemy!</div>
      }
      @if (error()) {
        <div class="notice error">{{ error() }}</div>
      }

      @if (confirming()) {
        <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
          <p class="muted">Potwierdzanie płatności…</p>
        </div>
      } @else {
        @if (auth.account(); as acc) {
        <div class="card stack">
          <div class="row between">
            <span class="muted">E-mail</span>
            <strong>{{ acc.email }}</strong>
          </div>
          <div class="row between">
            <span class="muted">Plan</span>
            <strong>
              @switch (acc.plan) {
                @case ('premium') { ⭐ Premium — 9 zł/mies. }
                @case ('trial') { Okres próbny — zostało {{ acc.trialDaysLeft }} dni }
                @case ('expired') { Okres próbny zakończony }
                @default { Konto nieaktywowane }
              }
            </strong>
          </div>
          @if (acc.plan === 'trial') {
            <p class="muted small">Pełny dostęp za darmo do {{ acc.trialEndsAt | date:'d.MM.y' }}.</p>
          }

          @if (acc.plan === 'free') {
            <div class="notice warn">
              Kliknij link aktywacyjny z maila, aby rozpocząć 30-dniowy darmowy okres próbny.
            </div>
          } @else if (acc.plan !== 'premium') {
            <button class="btn btn-primary btn-block" [disabled]="busy()" (click)="buy()">
              {{ busy() ? 'Przekierowywanie do płatności…' : 'Przejdź na Premium — 9 zł/mies.' }}
            </button>
            <p class="muted small center">Bezpieczna płatność przez Stripe. Anulujesz kiedy chcesz.</p>
          }
        </div>

        <div class="card stack">
          <button class="btn btn-ghost btn-block" (click)="logout()">Wyloguj się</button>
        </div>
        } @else {
          <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
            <p class="muted">Wczytywanie konta…</p>
          </div>
        }
      }
    </div>
  `
})
export class AccountComponent implements OnInit {
  auth = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  busy = signal(false);
  confirming = signal(false);
  error = signal<string | null>(null);
  paywall = signal(false);
  paymentCancelled = signal(false);
  paymentSuccess = signal(false);

  ngOnInit(): void {
    const qp = this.route.snapshot.queryParamMap;
    this.paywall.set(qp.has('paywall'));
    this.paymentCancelled.set(qp.get('payment') === 'cancel');

    const sessionId = qp.get('session_id');
    if (sessionId) {
      // powrót ze Stripe Checkout — potwierdzamy płatność bez czekania na webhook
      this.confirming.set(true);
      this.auth.confirmPayment(sessionId).subscribe({
        next: () => {
          this.confirming.set(false);
          this.paymentSuccess.set(true);
          this.router.navigate([], { queryParams: {}, replaceUrl: true });
        },
        error: err => {
          this.confirming.set(false);
          this.error.set(apiErrorMessage(err, 'Nie udało się potwierdzić płatności.'));
          this.auth.refreshAccount().subscribe({ error: () => {} });
        }
      });
      return;
    }

    this.auth.refreshAccount().subscribe({
      error: err => this.error.set(apiErrorMessage(err, 'Nie udało się pobrać danych konta.'))
    });
  }

  buy(): void {
    if (this.busy()) return;
    this.error.set(null);
    this.busy.set(true);
    this.auth.checkout().subscribe({
      next: res => {
        window.location.href = res.url;
      },
      error: err => {
        this.busy.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się rozpocząć płatności.'));
      }
    });
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}

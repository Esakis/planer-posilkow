import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Account } from '../core/models';
import { AuthService } from '../core/auth.service';
import { apiErrorMessage } from '../core/api-error';

/** Strona, na którą prowadzi link aktywacyjny z maila (/activate?token=...). */
@Component({
  selector: 'app-activate',
  standalone: true,
  imports: [RouterLink, DatePipe],
  template: `
    <div class="container stack" style="max-width:440px">
      @if (busy()) {
        <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
          <p class="muted">Aktywowanie konta…</p>
        </div>
      } @else {
        @if (account(); as acc) {
          <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
            <div style="font-size:40px">🎉</div>
            <h2 style="font-size:18px">Konto aktywne!</h2>
            <p class="muted">
              Masz pełny dostęp za darmo do
              <strong>{{ acc.trialEndsAt | date:'d.MM.y' }}</strong>
              ({{ acc.trialDaysLeft }} dni).
            </p>
            @if (loggedIn) {
              <a class="btn btn-primary" routerLink="/">Ułóż pierwszy plan</a>
            } @else {
              <a class="btn btn-primary" routerLink="/login">Zaloguj się</a>
            }
          </div>
        } @else {
          <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
            <div style="font-size:40px">😕</div>
            <div class="notice error">{{ error() }}</div>
            <a class="btn btn-ghost" routerLink="/register">Załóż konto</a>
          </div>
        }
      }
    </div>
  `
})
export class ActivateComponent implements OnInit {
  private auth = inject(AuthService);
  private route = inject(ActivatedRoute);

  busy = signal(true);
  account = signal<Account | null>(null);
  error = signal('Link aktywacyjny jest nieprawidłowy.');
  loggedIn = this.auth.isLoggedIn();

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.busy.set(false);
      return;
    }
    this.auth.activate(token).subscribe({
      next: acc => {
        this.account.set(acc);
        this.busy.set(false);
      },
      error: err => {
        this.error.set(apiErrorMessage(err, 'Link aktywacyjny jest nieprawidłowy lub został już użyty.'));
        this.busy.set(false);
      }
    });
  }
}

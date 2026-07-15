import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { apiErrorMessage } from '../core/api-error';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="container stack" style="max-width:420px;padding-top:36px">
      <div class="center">
        <div style="font-size:52px;line-height:1">🥦</div>
        <h1 style="font-size:24px;margin-top:10px">Witaj z powrotem!</h1>
        <p class="muted small" style="margin-top:4px">Zaloguj się i planuj tanie obiady na cały tydzień</p>
      </div>

      <div class="card stack" style="padding:22px">
        <label class="field" style="margin-bottom:0">
          <span class="lbl">E-mail</span>
          <input type="email" autocomplete="email" placeholder="ty@przyklad.pl" autofocus
                 [value]="email()" (input)="email.set($any($event.target).value)"
                 (keydown.enter)="submit()" />
        </label>
        <label class="field" style="margin-bottom:0">
          <span class="lbl">Hasło</span>
          <input type="password" autocomplete="current-password" placeholder="••••••••"
                 [value]="password()" (input)="password.set($any($event.target).value)"
                 (keydown.enter)="submit()" />
        </label>

        @if (error()) {
          <div class="notice error">{{ error() }}</div>
        }

        <button class="btn btn-primary btn-block" [disabled]="busy()" (click)="submit()">
          {{ busy() ? 'Logowanie…' : 'Zaloguj się' }}
        </button>
      </div>

      <p class="muted small center">
        Nie masz konta?
        <a routerLink="/register" style="color:var(--green-dark);font-weight:700">Załóż konto — 30 dni za darmo</a>
      </p>
    </div>
  `
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  email = signal('');
  password = signal('');
  busy = signal(false);
  error = signal<string | null>(null);

  submit(): void {
    if (this.busy()) return;
    this.error.set(null);
    this.busy.set(true);
    this.auth.login(this.email().trim(), this.password()).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: err => {
        this.busy.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się zalogować. Spróbuj ponownie.'));
      }
    });
  }
}

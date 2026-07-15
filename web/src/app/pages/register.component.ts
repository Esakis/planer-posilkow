import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { apiErrorMessage } from '../core/api-error';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="container stack" style="max-width:420px;padding-top:36px">
      @if (!sent()) {
        <div class="center">
          <div style="font-size:52px;line-height:1">🥦</div>
          <h1 style="font-size:24px;margin-top:10px">Załóż konto</h1>
          <p class="muted small" style="margin-top:4px">
            30 dni pełnego dostępu za darmo — bez karty.<br>Potem 9 zł/mies., jeśli zostaniesz.
          </p>
        </div>

        <div class="card stack" style="padding:22px">
          <label class="field" style="margin-bottom:0">
            <span class="lbl">E-mail</span>
            <input type="email" autocomplete="email" placeholder="ty@przyklad.pl" autofocus
                   [value]="email()" (input)="email.set($any($event.target).value)"
                   (keydown.enter)="submit()" />
          </label>
          <label class="field" style="margin-bottom:0">
            <span class="lbl">Hasło <span class="hint">(min. 8 znaków)</span></span>
            <input type="password" autocomplete="new-password" placeholder="••••••••"
                   [value]="password()" (input)="password.set($any($event.target).value)"
                   (keydown.enter)="submit()" />
          </label>

          @if (error()) {
            <div class="notice error">{{ error() }}</div>
          }

          <button class="btn btn-primary btn-block" [disabled]="busy()" (click)="submit()">
            {{ busy() ? 'Zakładanie konta…' : 'Załóż konto — 30 dni za darmo' }}
          </button>
        </div>

        <p class="muted small center">
          Masz już konto?
          <a routerLink="/login" style="color:var(--green-dark);font-weight:700">Zaloguj się</a>
        </p>
      } @else {
        <div class="card center stack" style="padding:32px 22px">
          <div style="font-size:44px;line-height:1">📬</div>
          <h2 style="font-size:19px">Sprawdź skrzynkę</h2>
          <p class="muted">
            Wysłaliśmy link aktywacyjny na <strong>{{ email() }}</strong>.
            Kliknij go, aby rozpocząć 30-dniowy darmowy okres próbny.
          </p>
          @if (devLink(); as link) {
            <div class="notice warn" style="text-align:left">
              <strong>Tryb deweloperski:</strong> mail nie został realnie wysłany.
              <a [href]="link" style="color:inherit">Kliknij tutaj, aby aktywować konto</a>.
            </div>
          }
          <a class="btn btn-ghost" routerLink="/login">Przejdź do logowania</a>
        </div>
      }
    </div>
  `
})
export class RegisterComponent {
  private auth = inject(AuthService);

  email = signal('');
  password = signal('');
  busy = signal(false);
  error = signal<string | null>(null);
  sent = signal(false);
  devLink = signal<string | null>(null);

  submit(): void {
    if (this.busy()) return;
    this.error.set(null);
    this.busy.set(true);
    this.auth.register(this.email().trim(), this.password()).subscribe({
      next: res => {
        this.busy.set(false);
        this.devLink.set(res.devActivationLink);
        this.sent.set(true);
      },
      error: err => {
        this.busy.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się założyć konta. Spróbuj ponownie.'));
      }
    });
  }
}

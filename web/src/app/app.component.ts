import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <header class="appbar">
      <div class="appbar-inner">
        <a class="brand" routerLink="/" style="color:#fff;text-decoration:none" (click)="menuOpen.set(false)">
          <span class="leaf">🥦</span> TaniTydzień
        </a>
        <span class="spacer"></span>
        <a class="linkbtn" routerLink="/recipes">Przepisy</a>
        <a class="linkbtn" routerLink="/my-list">Moja lista</a>
        <a class="linkbtn" routerLink="/history">Historia</a>
        <a class="linkbtn" routerLink="/">Nowy plan</a>
        <a class="linkbtn" [routerLink]="auth.isLoggedIn() ? '/account' : '/login'">Konto</a>
        <button class="menubtn" (click)="menuOpen.set(!menuOpen())" aria-label="Menu">
          {{ menuOpen() ? '✕' : '☰' }}
        </button>
      </div>
      @if (menuOpen()) {
        <nav class="mobilemenu" (click)="menuOpen.set(false)">
          <a routerLink="/" routerLinkActive="on" [routerLinkActiveOptions]="{ exact: true }">🥦 Nowy plan</a>
          <a routerLink="/recipes" routerLinkActive="on">🍲 Przepisy</a>
          <a routerLink="/my-list" routerLinkActive="on">🛒 Moja lista</a>
          <a routerLink="/history" routerLinkActive="on">🕘 Historia</a>
          <a [routerLink]="auth.isLoggedIn() ? '/account' : '/login'" routerLinkActive="on">👤 Konto</a>
        </nav>
      }
    </header>
    <router-outlet />
    <nav class="bottomnav">
      <a routerLink="/" routerLinkActive="on" [routerLinkActiveOptions]="{ exact: true }">
        <span class="ico">🥦</span>Plan
      </a>
      <a routerLink="/recipes" routerLinkActive="on"><span class="ico">🍲</span>Przepisy</a>
      <a routerLink="/my-list" routerLinkActive="on"><span class="ico">🛒</span>Lista</a>
      <a routerLink="/history" routerLinkActive="on"><span class="ico">🕘</span>Historia</a>
      <a [routerLink]="auth.isLoggedIn() ? '/account' : '/login'" routerLinkActive="on">
        <span class="ico">👤</span>Konto
      </a>
    </nav>
  `
})
export class AppComponent {
  /** Rozwijane menu w nagłówku — tylko na wąskich ekranach (patrz .menubtn w styles.css). */
  menuOpen = signal(false);

  auth = inject(AuthService);

  constructor() {
    // odśwież stan konta po powrocie do aplikacji; przy wygasłym tokenie interceptor wyloguje
    if (this.auth.isLoggedIn()) {
      this.auth.refreshAccount().subscribe({ error: () => {} });
    }
  }
}

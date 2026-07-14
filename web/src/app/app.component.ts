import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

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
        <a class="linkbtn" routerLink="/add-recipe">Własny przepis</a>
        <a class="linkbtn" routerLink="/my-list">Moja lista</a>
        <a class="linkbtn" routerLink="/history">Historia</a>
        <a class="linkbtn" routerLink="/">Nowy plan</a>
        <button class="menubtn" (click)="menuOpen.set(!menuOpen())" aria-label="Menu">
          {{ menuOpen() ? '✕' : '☰' }}
        </button>
      </div>
      @if (menuOpen()) {
        <nav class="mobilemenu" (click)="menuOpen.set(false)">
          <a routerLink="/" routerLinkActive="on" [routerLinkActiveOptions]="{ exact: true }">🥦 Nowy plan</a>
          <a routerLink="/add-recipe" routerLinkActive="on">📝 Własny przepis</a>
          <a routerLink="/my-list" routerLinkActive="on">🛒 Moja lista</a>
          <a routerLink="/history" routerLinkActive="on">🕘 Historia</a>
        </nav>
      }
    </header>
    <router-outlet />
    <nav class="bottomnav">
      <a routerLink="/" routerLinkActive="on" [routerLinkActiveOptions]="{ exact: true }">
        <span class="ico">🥦</span>Plan
      </a>
      <a routerLink="/add-recipe" routerLinkActive="on"><span class="ico">📝</span>Przepis</a>
      <a routerLink="/my-list" routerLinkActive="on"><span class="ico">🛒</span>Lista</a>
      <a routerLink="/history" routerLinkActive="on"><span class="ico">🕘</span>Historia</a>
    </nav>
  `
})
export class AppComponent {
  /** Rozwijane menu w nagłówku — tylko na wąskich ekranach (patrz .menubtn w styles.css). */
  menuOpen = signal(false);
}

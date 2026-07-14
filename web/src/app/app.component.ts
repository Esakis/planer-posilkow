import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <header class="appbar">
      <div class="appbar-inner">
        <a class="brand" routerLink="/" style="color:#fff;text-decoration:none">
          <span class="leaf">🥦</span> TaniTydzień
        </a>
        <span class="spacer"></span>
        <a class="linkbtn" routerLink="/add-recipe">Własny przepis</a>
        <a class="linkbtn" routerLink="/my-list">Moja lista</a>
        <a class="linkbtn" routerLink="/history">Historia</a>
        <a class="linkbtn" routerLink="/">Nowy plan</a>
      </div>
    </header>
    <router-outlet />
  `
})
export class AppComponent {}

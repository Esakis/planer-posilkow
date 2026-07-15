import { Component } from '@angular/core';

/** Legenda kolorów cen — spójna z klasami .price-* w styles.css. */
@Component({
  selector: 'app-price-legend',
  standalone: true,
  template: `
    <div class="muted small" style="display:flex;gap:4px 14px;flex-wrap:wrap">
      <span><b style="color:var(--ink)">●</b> z gazetki (zweryfikowana)</span>
      <span><b style="color:#1a5dbf">●</b> wpisana przez użytkownika</span>
      <span><b style="color:var(--muted)">●</b> przewidywana — kliknij cenę, aby ją poprawić</span>
    </div>
  `
})
export class PriceLegendComponent {}

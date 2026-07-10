import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { PlanStateService } from '../core/plan-state.service';
import { CompareResponse, StoreCost, StoreName } from '../core/models';

const STORE_EMOJI: Record<StoreName, string> = {
  Biedronka: '🐞',
  Lidl: '🔵',
  Auchan: '🦜'
};

@Component({
  selector: 'app-compare',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Gdzie kupić najtaniej?</h1>
        <a class="btn btn-ghost btn-sm" routerLink="/menu">← Jadłospis</a>
      </div>

      @if (loading()) {
        <div class="card center"><div class="spinner"></div><p class="muted small" style="margin-top:10px">Porównuję ceny w sklepach…</p></div>
      } @else if (error()) {
        <div class="notice error">{{ error() }}</div>
        <button class="btn btn-primary" routerLink="/menu">Wróć do jadłospisu</button>
      } @else {
        @if (data(); as d) {
        <div class="stack">

          <p class="muted small">Ten sam jadłospis, wyceniony w każdej sieci (ceny szacunkowe, z aktualnymi promocjami).
            Wybierz sklep — do niego przygotujemy listę zakupów.</p>

          @if (d.maxSaving > 0) {
            <div class="card center" style="background:var(--green-soft);border-color:#bfe0cc">
              <div class="muted small">Wybierając najtańszy sklep zaoszczędzisz nawet</div>
              <div style="font-size:30px;font-weight:800;color:var(--green-dark)" class="mono-price">{{ d.maxSaving | number:'1.2-2' }} zł</div>
              <div class="muted small">na tym tygodniu</div>
            </div>
          }

          @for (s of d.stores; track s.store; let i = $index) {
            <div class="card" [style.border-color]="s.cheapest ? 'var(--green)' : 'var(--line)'"
                 [style.border-width]="s.cheapest ? '2px' : '1px'">
              <div class="row between" style="align-items:flex-start;gap:12px">
                <div style="flex:1">
                  <div class="row" style="gap:8px">
                    <span style="font-size:22px">{{ emoji(s.store) }}</span>
                    <div>
                      <div class="row" style="gap:8px;align-items:center">
                        <span style="font-size:18px;font-weight:800">{{ s.store }}</span>
                        @if (s.cheapest) { <span class="tag" style="background:var(--green);color:#fff">🏆 najtaniej</span> }
                        @else { <span class="chip" style="padding:2px 8px">+{{ s.diffToCheapest | number:'1.2-2' }} zł</span> }
                      </div>
                      <div class="muted small" style="margin-top:2px">
                        {{ s.kind === 'hipermarket' ? '🚗 ' : '🚶 ' }}{{ s.note }}
                      </div>
                    </div>
                  </div>
                </div>
                <div style="text-align:right">
                  <div class="mono-price" style="font-size:22px;font-weight:800">{{ s.total | number:'1.2-2' }} zł</div>
                  @if (s.promoSavings > 0) {
                    <div class="small" style="color:var(--promo);font-weight:700">
                      {{ s.promoItems }} promocji · −{{ s.promoSavings | number:'1.2-2' }} zł
                    </div>
                  } @else {
                    <div class="muted small">brak promocji</div>
                  }
                </div>
              </div>

              <button class="btn btn-block" style="margin-top:14px"
                      [class.btn-primary]="s.cheapest" [class.btn-ghost]="!s.cheapest"
                      (click)="choose(s)">
                {{ s.cheapest ? '🛒 Kupuję tutaj — pokaż listę' : 'Wybieram ' + s.store }}
              </button>
            </div>
          }

          <p class="muted small center">
            💡 Jadłospis był ułożony pod {{ generatedStore() }} — dlatego często ten sklep wypada
            najkorzystniej. Chcesz plan pod inny sklep? Ułóż go od nowa, wybierając ten sklep.
          </p>
        </div>
        }
      }
    </div>
  `
})
export class CompareComponent {
  private api = inject(ApiService);
  private state = inject(PlanStateService);
  private router = inject(Router);

  data = signal<CompareResponse | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  generatedStore = signal<string>('');

  constructor() {
    const ob = this.state.onboarding();
    const ids = this.state.recipeIds();
    if (!ob || ids.length === 0) {
      this.loading.set(false);
      this.error.set('Brak jadłospisu — wróć i ułóż plan.');
      return;
    }
    this.generatedStore.set(ob.store);
    this.api.compare({ recipeIds: ids, people: ob.people }).subscribe({
      next: d => { this.data.set(d); this.loading.set(false); },
      error: err => {
        this.loading.set(false);
        this.error.set(err?.error?.message ?? 'Nie udało się porównać sklepów.');
      }
    });
  }

  emoji(store: StoreName): string { return STORE_EMOJI[store] ?? '🛒'; }

  choose(s: StoreCost): void {
    this.state.setStore(s.store);
    this.router.navigate(['/shopping']);
  }
}

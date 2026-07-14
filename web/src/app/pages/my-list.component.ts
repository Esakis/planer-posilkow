import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { readJson, writeJson } from '../core/storage';
import { CompareResponse, Ingredient, ShoppingList, StoreName } from '../core/models';

const STORAGE_KEY = 'tanitydzien.mylist.v1';

const STORE_EMOJI: Record<StoreName, string> = {
  Biedronka: '🐞',
  Lidl: '🔵',
  Auchan: '🦜'
};

/** Pozycja listy trzymana w localStorage — tyle, ile trzeba do wyświetlenia i wyceny. */
interface MyListItem {
  id: number;
  name: string;
  aisle: string;
  grams: number;
}

@Component({
  selector: 'app-my-list',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Moja lista zakupów</h1>
        <a class="btn btn-ghost btn-sm" routerLink="/">← Start</a>
      </div>

      <p class="muted small">Dodaj produkty, a porównamy koszt całej listy w Biedronce, Lidlu i Auchan
        — z aktualnymi promocjami.</p>

      @if (loadError()) {
        <div class="notice error">{{ loadError() }}</div>
      } @else {

      <div class="card stack">
        @if (items().length === 0) {
          <div class="muted small center">Lista jest pusta — wyszukaj pierwszy produkt poniżej.</div>
        }
        @for (row of items(); track row.id; let i = $index) {
          <div class="row between" style="gap:10px;border-top:1px solid var(--line);padding-top:8px">
            <div style="flex:1">
              <div style="font-weight:700">{{ row.name }}</div>
              <div class="muted small">{{ row.aisle }}</div>
            </div>
            <input type="number" min="1" max="50000" style="width:90px" [value]="row.grams"
                   (input)="setGrams(i, +$any($event.target).value)" />
            <span class="muted small" style="align-self:center">g</span>
            <button class="btn btn-ghost btn-sm" (click)="removeItem(i)">✕</button>
          </div>
        }

        <div>
          <input type="text" placeholder="🔍 Szukaj produktu…" [value]="search()"
                 (input)="search.set($any($event.target).value)" />
          @if (search().trim().length > 0) {
            @if (matches().length > 0) {
              <div class="stack" style="--space:0;margin-top:6px">
                @for (ing of matches(); track ing.id) {
                  <button class="btn btn-ghost btn-sm" style="justify-content:flex-start;text-align:left"
                          (click)="addItem(ing)">
                    + {{ ing.name }} <span class="muted small">({{ ing.aisle }})</span>
                  </button>
                }
              </div>
            } @else {
              <div class="muted small" style="margin-top:6px">
                Nie znamy cen tego produktu — na razie porównujemy tylko produkty z naszej bazy.
              </div>
            }
          }
        </div>
      </div>

      <button class="btn btn-primary btn-block" [disabled]="items().length === 0 || comparing()"
              (click)="compare()">
        {{ comparing() ? 'Porównuję…' : '💰 Gdzie najtaniej?' }}
      </button>

      @if (error()) {
        <div class="notice error">{{ error() }}</div>
      }

      @if (result(); as d) {
        <div class="stack">
          @if (d.maxSaving > 0) {
            <div class="card center" style="background:var(--green-soft);border-color:#bfe0cc">
              <div class="muted small">Wybierając najtańszy sklep zaoszczędzisz nawet</div>
              <div style="font-size:30px;font-weight:800;color:var(--green-dark)" class="mono-price">{{ d.maxSaving | number:'1.2-2' }} zł</div>
              <div class="muted small">na tej liście</div>
            </div>
          }

          @for (s of d.stores; track s.store) {
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
                      [disabled]="listLoading()"
                      (click)="showList(s.store)">
                {{ detailStore() === s.store ? '✓ Lista poniżej' : 'Pokaż listę (' + s.store + ')' }}
              </button>
            </div>
          }
        </div>
      }

      @if (listLoading()) {
        <div class="card center"><div class="spinner"></div><p class="muted small" style="margin-top:10px">Liczę koszyk…</p></div>
      }
      @if (!listLoading() && detail(); as l) {
        <div class="stack">
          <div class="card" style="background:var(--green-soft);border-color:#bfe0cc">
            <div class="muted small">Razem ({{ l.store }})</div>
            <div style="font-size:26px;font-weight:800" class="mono-price">{{ l.total | number:'1.2-2' }} zł</div>
            @if (l.promoSavings > 0) {
              <div class="small" style="color:var(--promo);font-weight:700">na promocjach −{{ l.promoSavings | number:'1.2-2' }} zł</div>
            }
          </div>

          @for (g of l.groups; track g.aisle) {
            <div class="card">
              <div class="row between" style="margin-bottom:10px">
                <h2 style="font-size:15px;text-transform:uppercase;letter-spacing:.4px;color:var(--muted)">{{ g.aisle }}</h2>
                <span class="mono-price muted small">{{ g.subtotal | number:'1.2-2' }} zł</span>
              </div>
              <div class="stack" style="--space:0">
                @for (it of g.items; track it.ingredient) {
                  <div class="row between" style="gap:12px;padding:8px 0;border-top:1px solid var(--line)">
                    <div>
                      <div style="font-weight:700">{{ it.ingredient }}</div>
                      <div class="muted small">{{ it.displayQty }}
                        @if (it.onPromo && it.promoNote) { · <span style="color:var(--promo);font-weight:700">🏷️ {{ it.promoNote }}</span> }
                      </div>
                    </div>
                    <span class="mono-price" style="font-weight:700;white-space:nowrap">{{ it.cost | number:'1.2-2' }} zł</span>
                  </div>
                }
              </div>
            </div>
          }
        </div>
      }

      }
    </div>
  `
})
export class MyListComponent {
  private api = inject(ApiService);

  ingredients = signal<Ingredient[]>([]);
  loadError = signal<string | null>(null);

  items = signal<MyListItem[]>(readJson<MyListItem[]>(STORAGE_KEY) ?? []);
  search = signal('');

  comparing = signal(false);
  error = signal<string | null>(null);
  result = signal<CompareResponse | null>(null);

  listLoading = signal(false);
  detailStore = signal<StoreName | null>(null);
  detail = signal<ShoppingList | null>(null);

  matches = computed(() => {
    const q = this.search().trim().toLowerCase();
    if (!q) return [];
    const usedIds = new Set(this.items().map(r => r.id));
    return this.ingredients()
      .filter(i => !usedIds.has(i.id) && i.name.toLowerCase().includes(q))
      .slice(0, 8);
  });

  constructor() {
    this.api.ingredients().subscribe({
      next: list => this.ingredients.set(list),
      error: err => this.loadError.set(apiErrorMessage(err, 'Nie udało się pobrać produktów. Sprawdź, czy API działa.'))
    });
  }

  emoji(store: StoreName): string { return STORE_EMOJI[store] ?? '🛒'; }

  addItem(ing: Ingredient): void {
    this.updateItems(rows => [...rows, { id: ing.id, name: ing.name, aisle: ing.aisle, grams: 500 }]);
    this.search.set('');
  }

  removeItem(index: number): void {
    this.updateItems(rows => rows.filter((_, i) => i !== index));
  }

  setGrams(index: number, grams: number): void {
    this.updateItems(rows => rows.map((r, i) => i === index ? { ...r, grams } : r));
  }

  compare(): void {
    const items = this.items().filter(r => r.grams >= 1);
    if (items.length === 0 || this.comparing()) return;
    this.comparing.set(true);
    this.error.set(null);
    this.api.customCompare({ items: items.map(r => ({ ingredientId: r.id, grams: r.grams })) }).subscribe({
      next: d => { this.result.set(d); this.comparing.set(false); },
      error: err => {
        this.comparing.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się porównać sklepów.'));
      }
    });
  }

  showList(store: StoreName): void {
    const items = this.items().filter(r => r.grams >= 1);
    if (items.length === 0 || this.listLoading()) return;
    this.listLoading.set(true);
    this.detailStore.set(store);
    this.error.set(null);
    this.api.customList({ items: items.map(r => ({ ingredientId: r.id, grams: r.grams })), store }).subscribe({
      next: l => { this.detail.set(l); this.listLoading.set(false); },
      error: err => {
        this.listLoading.set(false);
        this.detailStore.set(null);
        this.error.set(apiErrorMessage(err, 'Nie udało się pobrać listy.'));
      }
    });
  }

  /** Każda zmiana listy zapisuje ją i unieważnia poprzednie wyniki porównania. */
  private updateItems(fn: (rows: MyListItem[]) => MyListItem[]): void {
    this.items.update(fn);
    writeJson(STORAGE_KEY, this.items());
    this.result.set(null);
    this.detail.set(null);
    this.detailStore.set(null);
  }
}

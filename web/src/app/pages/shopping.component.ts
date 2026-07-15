import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { apiErrorMessage, isNetworkError } from '../core/api-error';
import { PlanStateService } from '../core/plan-state.service';
import { readJson, writeJson } from '../core/storage';
import { IngredientLine, ShoppingList, StoreName } from '../core/models';
import { PriceLegendComponent } from '../shared/price-legend.component';
import { PriceEditorComponent } from '../shared/price-editor.component';

/** Ostatnio pobrana lista (jedna — dla bieżącego planu) do użycia offline w sklepie. */
const CACHE_KEY = 'tanitydzien.shopping.v1';

interface CachedList {
  planKey: string;
  list: ShoppingList;
}

@Component({
  selector: 'app-shopping',
  standalone: true,
  imports: [RouterLink, DecimalPipe, PriceLegendComponent, PriceEditorComponent],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Lista zakupów</h1>
        <a class="btn btn-ghost btn-sm" routerLink="/menu">← Jadłospis</a>
      </div>

      @if (loading()) {
        <div class="card center"><div class="spinner"></div><p class="muted small" style="margin-top:10px">Liczę koszyk…</p></div>
      } @else if (error()) {
        <div class="notice error">{{ error() }}</div>
        <button class="btn btn-primary" routerLink="/menu">Wróć do jadłospisu</button>
      } @else {
        @if (offline()) {
          <div class="notice warn">📴 Brak połączenia — pokazuję ostatnio pobraną listę.</div>
        }
        @if (list(); as l) {
        <div class="stack">

        <div class="card" style="background:var(--green-soft);border-color:#bfe0cc">
          <div class="row between">
            <div>
              <div class="muted small">Razem ({{ l.store }})</div>
              <div style="font-size:26px;font-weight:800" class="mono-price">{{ l.total | number:'1.2-2' }} zł</div>
            </div>
            @if (l.promoSavings > 0) {
              <div style="text-align:right">
                <div class="muted small">na promocjach</div>
                <div style="color:var(--promo);font-weight:800" class="mono-price">−{{ l.promoSavings | number:'1.2-2' }} zł</div>
              </div>
            }
          </div>
          <div class="muted small" style="margin-top:8px">
            Odhaczono {{ checkedCount() }} z {{ totalItems() }}
          </div>
          <div style="margin-top:8px"><app-price-legend /></div>
        </div>

        @for (g of l.groups; track g.aisle) {
          <div class="card">
            <div class="row between" style="margin-bottom:10px">
              <h2 style="font-size:15px;text-transform:uppercase;letter-spacing:.4px;color:var(--muted)">{{ g.aisle }}</h2>
              <span class="mono-price muted small">{{ g.subtotal | number:'1.2-2' }} zł</span>
            </div>
            <div class="stack" style="--space:0">
              @for (it of g.items; track it.ingredient) {
                <label class="row between" style="gap:12px;cursor:pointer;padding:8px 0;border-top:1px solid var(--line)">
                  <div class="row" style="gap:12px;align-items:flex-start">
                    <input type="checkbox" style="width:20px;height:20px;margin-top:2px;accent-color:var(--green)"
                           [checked]="isChecked(it.ingredient)" (change)="toggle(it.ingredient)" />
                    <div [style.opacity]="isChecked(it.ingredient) ? .5 : 1"
                         [style.text-decoration]="isChecked(it.ingredient) ? 'line-through' : 'none'">
                      <div style="font-weight:700">{{ it.ingredient }}</div>
                      <div class="muted small">{{ it.displayQty }}
                        @if (it.onPromo && it.promoNote) { · <span style="color:var(--promo);font-weight:700">🏷️ {{ it.promoNote }}</span> }
                      </div>
                    </div>
                  </div>
                  <span class="mono-price" style="font-weight:700;white-space:nowrap"
                        [class.price-predicted]="it.source === 'predicted'"
                        [class.price-user]="it.source === 'user'"
                        [class.price-editable]="it.source !== 'verified'"
                        (click)="editPrice(it, $event)">{{ it.cost | number:'1.2-2' }} zł</span>
                </label>
                @if (editing() === it.ingredientId) {
                  <app-price-editor [ingredientId]="it.ingredientId" [ingredientName]="it.ingredient"
                                    [initialStore]="currentStore()"
                                    (saved)="onPriceSaved()" (cancelled)="editing.set(null)" />
                }
              }
            </div>
          </div>
        }

        <button class="btn btn-ghost btn-block" (click)="reset()">Odznacz wszystko</button>
        </div>
        }
      }
    </div>
  `
})
export class ShoppingComponent {
  private api = inject(ApiService);
  private state = inject(PlanStateService);

  list = signal<ShoppingList | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  offline = signal(false);
  checked = signal<Set<string>>(new Set());
  /** Id składnika, którego cena jest właśnie edytowana. */
  editing = signal<number | null>(null);

  constructor() {
    this.load();
  }

  private load(): void {
    const ob = this.state.onboarding();
    const ids = this.state.recipeIds();
    if (!ob || ids.length === 0) {
      this.loading.set(false);
      this.error.set('Brak jadłospisu — wróć i ułóż plan.');
      return;
    }
    this.loading.set(true);
    const planKey = `${ob.store}:${ob.people}:${[...ids].sort((a, b) => a - b).join(',')}`;
    this.api.shoppingList({ recipeIds: ids, people: ob.people, store: ob.store }).subscribe({
      next: l => {
        this.list.set(l);
        this.loading.set(false);
        writeJson(CACHE_KEY, { planKey, list: l } satisfies CachedList);
      },
      error: err => {
        this.loading.set(false);
        // offline w sklepie — spróbuj ostatnio pobranej listy dla tego samego planu;
        // błędy serwera (4xx/5xx) pokazujemy normalnie, nie maskujemy ich cache'em
        const cached = isNetworkError(err) ? readJson<CachedList>(CACHE_KEY) : null;
        if (cached?.planKey === planKey && cached.list) {
          this.list.set(cached.list);
          this.offline.set(true);
        } else {
          this.error.set(apiErrorMessage(err, 'Nie udało się pobrać listy zakupów.'));
        }
      }
    });
  }

  currentStore(): StoreName {
    return this.state.onboarding()?.store ?? 'Biedronka';
  }

  /** Klik w szarą/niebieską cenę otwiera edytor; gazetkowej (czarnej) nie edytujemy. */
  editPrice(it: IngredientLine, ev: Event): void {
    // cena leży w <label> checkboxa — bez preventDefault klik odhaczałby pozycję
    ev.preventDefault();
    ev.stopPropagation();
    if (it.source === 'verified') return;
    this.editing.set(this.editing() === it.ingredientId ? null : it.ingredientId);
  }

  onPriceSaved(): void {
    this.editing.set(null);
    this.load();
  }

  isChecked(key: string): boolean { return this.checked().has(key); }

  toggle(key: string): void {
    this.checked.update(set => {
      const next = new Set(set);
      next.has(key) ? next.delete(key) : next.add(key);
      return next;
    });
  }

  reset(): void { this.checked.set(new Set()); }

  checkedCount(): number { return this.checked().size; }

  totalItems(): number {
    return this.list()?.groups.reduce((n, g) => n + g.items.length, 0) ?? 0;
  }
}

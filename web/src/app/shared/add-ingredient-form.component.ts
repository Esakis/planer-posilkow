import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { Ingredient, IngredientPriceInput, StoreName } from '../core/models';

/** Działy zgodne z listą znaną API (IngredientsController.Aisles). */
const AISLES = ['Warzywa', 'Owoce', 'Mięso', 'Nabiał', 'Produkty sypkie', 'Konserwy', 'Tłuszcze', 'Przyprawy', 'Inne'];

const STORES: { name: StoreName; emoji: string }[] = [
  { name: 'Biedronka', emoji: '🐞' },
  { name: 'Lidl', emoji: '🔵' },
  { name: 'Auchan', emoji: '🦜' }
];

/**
 * Formularz dodania brakującego produktu do bazy — używany w /add-recipe i /my-list,
 * gdy wyszukiwarka składników nic nie znajdzie.
 */
@Component({
  selector: 'app-add-ingredient-form',
  standalone: true,
  template: `
    <div class="card stack" style="border-color:var(--green)">
      <div class="row between">
        <div style="font-weight:700">➕ Nowy produkt</div>
        <button class="btn btn-ghost btn-sm" (click)="cancelled.emit()">✕</button>
      </div>

      <div>
        <label for="ing-name" style="font-weight:700">Nazwa</label>
        <input id="ing-name" type="text" maxlength="60" [value]="name()"
               (input)="name.set($any($event.target).value)" placeholder="np. Tofu naturalne" />
      </div>

      <div class="row" style="gap:12px;flex-wrap:wrap">
        <div style="flex:1;min-width:150px">
          <label for="ing-aisle" style="font-weight:700">Dział sklepu</label>
          <select id="ing-aisle" [value]="aisle()" (change)="aisle.set($any($event.target).value)">
            @for (a of aisles; track a) { <option [value]="a">{{ a }}</option> }
          </select>
        </div>
        <div style="width:150px">
          <label for="ing-pack" style="font-weight:700">Opakowanie (g)</label>
          <input id="ing-pack" type="number" min="1" max="50000" [value]="packSizeG()"
                 (input)="packSizeG.set(+$any($event.target).value)" />
        </div>
      </div>

      <div>
        <div style="font-weight:700;margin-bottom:6px">Cena opakowania (zł) — wypełnij, gdzie znasz</div>
        <div class="stack" style="--space:8px">
          @for (s of stores; track s.name) {
            <div class="row" style="gap:10px">
              <span style="width:110px;font-weight:600">{{ s.emoji }} {{ s.name }}</span>
              <input type="number" min="0" step="0.01" style="flex:1" placeholder="—"
                     [value]="prices()[s.name] ?? ''"
                     (input)="setPrice(s.name, $any($event.target).value)" />
            </div>
          }
        </div>
      </div>

      <details>
        <summary style="font-weight:700;cursor:pointer">Wartości odżywcze na 100 g (opcjonalnie)</summary>
        <div class="row" style="gap:10px;flex-wrap:wrap;margin-top:10px">
          <div style="width:110px">
            <label class="muted small" for="ing-kcal">kcal</label>
            <input id="ing-kcal" type="number" min="0" max="900" [value]="kcal100() ?? ''"
                   (input)="kcal100.set(numOrNull($any($event.target).value))" />
          </div>
          <div style="width:110px">
            <label class="muted small" for="ing-prot">białko (g)</label>
            <input id="ing-prot" type="number" min="0" max="100" [value]="protein100() ?? ''"
                   (input)="protein100.set(numOrNull($any($event.target).value))" />
          </div>
          <div style="width:110px">
            <label class="muted small" for="ing-carb">węgle (g)</label>
            <input id="ing-carb" type="number" min="0" max="100" [value]="carbs100() ?? ''"
                   (input)="carbs100.set(numOrNull($any($event.target).value))" />
          </div>
          <div style="width:110px">
            <label class="muted small" for="ing-fat">tłuszcz (g)</label>
            <input id="ing-fat" type="number" min="0" max="100" [value]="fat100() ?? ''"
                   (input)="fat100.set(numOrNull($any($event.target).value))" />
          </div>
        </div>
      </details>

      @if (error()) {
        <div class="notice error">{{ error() }}</div>
      }

      <button class="btn btn-primary btn-block" [disabled]="saving() || !valid()" (click)="save()">
        {{ saving() ? 'Zapisuję…' : 'Zapisz produkt' }}
      </button>
      @if (!valid()) {
        <div class="muted small center">Podaj nazwę (min. 2 znaki), gramaturę opakowania i cenę w co najmniej jednym sklepie.</div>
      }
    </div>
  `
})
export class AddIngredientFormComponent {
  private api = inject(ApiService);

  /** Nazwa startowa — z frazy wpisanej w wyszukiwarkę rodzica. */
  initialName = input('');
  created = output<Ingredient>();
  cancelled = output<void>();

  readonly aisles = AISLES;
  readonly stores = STORES;

  name = signal('');
  aisle = signal('Inne');
  packSizeG = signal(500);
  prices = signal<Partial<Record<StoreName, number>>>({});
  kcal100 = signal<number | null>(null);
  protein100 = signal<number | null>(null);
  carbs100 = signal<number | null>(null);
  fat100 = signal<number | null>(null);

  saving = signal(false);
  error = signal<string | null>(null);

  valid = computed(() =>
    this.name().trim().length >= 2 &&
    this.packSizeG() >= 1 &&
    Object.values(this.prices()).some(p => p != null && p > 0));

  constructor() {
    // rodzic trzyma initialName niezmienne, póki formularz jest otwarty,
    // więc efekt nie nadpisze tego, co użytkownik dopisał ręcznie
    effect(() => this.name.set(this.initialName()));
  }

  numOrNull(raw: string): number | null {
    return raw === '' ? null : +raw;
  }

  setPrice(store: StoreName, raw: string): void {
    this.prices.update(p => ({ ...p, [store]: raw === '' ? undefined : +raw }));
  }

  save(): void {
    if (!this.valid() || this.saving()) return;
    this.saving.set(true);
    this.error.set(null);

    const prices: IngredientPriceInput[] = this.stores
      .map(s => ({ store: s.name, basePrice: this.prices()[s.name] ?? 0, packSizeG: this.packSizeG() }))
      .filter(p => p.basePrice > 0);

    this.api.createIngredient({
      name: this.name().trim(),
      aisle: this.aisle(),
      kcal100: this.kcal100(),
      protein100: this.protein100(),
      carbs100: this.carbs100(),
      fat100: this.fat100(),
      prices
    }).subscribe({
      next: ing => this.created.emit(ing),
      error: err => {
        this.saving.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się zapisać produktu.'));
      }
    });
  }
}

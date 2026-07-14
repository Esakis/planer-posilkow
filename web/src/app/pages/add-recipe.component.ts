import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { Ingredient } from '../core/models';
import { AddIngredientFormComponent } from '../shared/add-ingredient-form.component';

/** Tagi do wyboru — spójne z wykluczeniami w onboardingu i tagami seedów. */
const TAG_OPTIONS = ['wege', 'mięso', 'wieprzowina', 'ryby', 'lubiane-przez-dzieci'];

interface RecipeItemRow {
  ing: Ingredient;
  grams: number;
}

@Component({
  selector: 'app-add-recipe',
  standalone: true,
  imports: [RouterLink, DecimalPipe, AddIngredientFormComponent],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Dodaj własny przepis</h1>
        <a class="btn btn-ghost btn-sm" routerLink="/">← Start</a>
      </div>

      @if (loadError()) {
        <div class="notice error">{{ loadError() }}</div>
      } @else {

      <div class="card stack">
        <div>
          <label for="name" style="font-weight:700">Nazwa przepisu</label>
          <input id="name" type="text" maxlength="80" [value]="name()"
                 (input)="name.set($any($event.target).value)" placeholder="np. Kotlety mielone babci" />
        </div>

        <div class="row" style="gap:16px;flex-wrap:wrap">
          <div>
            <label for="time" style="font-weight:700">Czas (min)</label>
            <input id="time" type="number" min="1" max="600" style="width:90px" [value]="timeMin()"
                   (input)="timeMin.set(+$any($event.target).value)" />
          </div>
          <div>
            <label for="servings" style="font-weight:700">Liczba porcji</label>
            <input id="servings" type="number" min="1" max="12" style="width:90px" [value]="servings()"
                   (input)="servings.set(+$any($event.target).value)" />
          </div>
        </div>

        <div>
          <div style="font-weight:700;margin-bottom:6px">Tagi</div>
          <div class="row" style="gap:8px;flex-wrap:wrap">
            @for (t of tagOptions; track t) {
              <span class="chip selectable" [class.on]="hasTag(t)" (click)="toggleTag(t)">{{ t }}</span>
            }
            @for (t of customTags(); track t) {
              <span class="chip selectable on" (click)="toggleTag(t)">{{ t }} ✕</span>
            }
          </div>
          <div class="row" style="gap:8px;margin-top:8px">
            <input type="text" maxlength="30" style="flex:1" placeholder="własny tag, np. na-imprezę"
                   [value]="newTag()" (input)="newTag.set($any($event.target).value)"
                   (keydown.enter)="addTag()" />
            <button class="btn btn-ghost btn-sm" (click)="addTag()">Dodaj tag</button>
          </div>
          <div class="muted small" style="margin-top:4px">
            Oznacz np. „mięso" albo „ryby" — dzięki temu wykluczenia w generatorze zadziałają też na Twój przepis.
          </div>
        </div>
      </div>

      <div class="card stack">
        <div style="font-weight:700">Składniki (ilości na cały przepis)</div>

        @for (row of items(); track row.ing.id; let i = $index) {
          <div class="row between" style="gap:10px;border-top:1px solid var(--line);padding-top:8px">
            <div style="flex:1">
              <div style="font-weight:700">{{ row.ing.name }}</div>
              <div class="muted small">{{ row.ing.aisle }} · {{ row.ing.kcal100 | number:'1.0-0' }} kcal/100 g</div>
            </div>
            <input type="number" min="1" max="20000" style="width:90px" [value]="row.grams"
                   (input)="setGrams(i, +$any($event.target).value)" />
            <span class="muted small" style="align-self:center">g</span>
            <button class="btn btn-ghost btn-sm" (click)="removeItem(i)">✕</button>
          </div>
        }

        <div>
          <input type="text" placeholder="🔍 Szukaj składnika…" [value]="search()"
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
              <div class="muted small" style="margin-top:6px">Brak takiego składnika w bazie.</div>
              @if (!showAddForm()) {
                <button class="btn btn-ghost btn-sm" style="margin-top:6px" (click)="openAddForm()">
                  ➕ Dodaj „{{ search().trim() }}" jako nowy produkt
                </button>
              }
            }
          }
        </div>

        @if (showAddForm()) {
          <app-add-ingredient-form [initialName]="newProductName()"
                                   (created)="onIngredientCreated($event)"
                                   (cancelled)="showAddForm.set(false)" />
        }
      </div>

      <div class="card stack">
        <label for="steps" style="font-weight:700">Kroki przygotowania (każdy w nowej linii)</label>
        <textarea id="steps" rows="6" [value]="stepsText()"
                  (input)="stepsText.set($any($event.target).value)"
                  placeholder="Obierz i pokrój warzywa.&#10;Podsmaż na oleju.&#10;Dodaj resztę składników i duś 20 min."></textarea>
      </div>

      @if (items().length > 0) {
        <div class="card" style="background:var(--green-soft);border-color:#bfe0cc">
          <div class="muted small">Makro na porcję (wyliczone ze składników)</div>
          <div class="row" style="gap:16px;margin-top:4px;font-weight:700">
            <span>{{ macro().kcal | number:'1.0-0' }} kcal</span>
            <span>B: {{ macro().protein | number:'1.0-1' }} g</span>
            <span>W: {{ macro().carbs | number:'1.0-1' }} g</span>
            <span>T: {{ macro().fat | number:'1.0-1' }} g</span>
          </div>
        </div>
      }

      @if (error()) {
        <div class="notice error">{{ error() }}</div>
      }

      <button class="btn btn-primary btn-block" [disabled]="saving() || !valid()" (click)="save()">
        {{ saving() ? 'Zapisuję…' : '💾 Zapisz przepis' }}
      </button>
      @if (!valid()) {
        <div class="muted small center">Uzupełnij nazwę (min. 3 znaki), co najmniej 1 składnik i 1 krok.</div>
      }

      }
    </div>
  `
})
export class AddRecipeComponent {
  private api = inject(ApiService);
  private router = inject(Router);

  readonly tagOptions = TAG_OPTIONS;

  ingredients = signal<Ingredient[]>([]);
  loadError = signal<string | null>(null);

  name = signal('');
  timeMin = signal(30);
  servings = signal(4);
  tags = signal<Set<string>>(new Set());
  newTag = signal('');
  stepsText = signal('');
  items = signal<RecipeItemRow[]>([]);
  search = signal('');
  showAddForm = signal(false);
  newProductName = signal('');

  saving = signal(false);
  error = signal<string | null>(null);

  matches = computed(() => {
    const q = this.search().trim().toLowerCase();
    if (!q) return [];
    const usedIds = new Set(this.items().map(r => r.ing.id));
    return this.ingredients()
      .filter(i => !usedIds.has(i.id) && i.name.toLowerCase().includes(q))
      .slice(0, 8);
  });

  macro = computed(() => {
    const servings = Math.max(1, this.servings());
    let p = 0, c = 0, f = 0, k = 0;
    for (const { ing, grams } of this.items()) {
      const perServing = (grams || 0) / servings / 100;
      p += perServing * ing.protein100;
      c += perServing * ing.carbs100;
      f += perServing * ing.fat100;
      k += perServing * ing.kcal100;
    }
    return { protein: p, carbs: c, fat: f, kcal: k };
  });

  valid = computed(() =>
    this.name().trim().length >= 3 &&
    this.timeMin() >= 1 && this.timeMin() <= 600 &&
    this.servings() >= 1 && this.servings() <= 12 &&
    this.items().length > 0 &&
    this.items().every(r => r.grams >= 1) &&
    this.steps().length > 0);

  constructor() {
    this.api.ingredients().subscribe({
      next: list => this.ingredients.set(list),
      error: err => this.loadError.set(apiErrorMessage(err, 'Nie udało się pobrać składników. Sprawdź, czy API działa.'))
    });
  }

  hasTag(t: string): boolean { return this.tags().has(t); }

  toggleTag(t: string): void {
    this.tags.update(set => {
      const next = new Set(set);
      next.has(t) ? next.delete(t) : next.add(t);
      return next;
    });
  }

  customTags = computed(() => [...this.tags()].filter(t => !TAG_OPTIONS.includes(t)));

  addTag(): void {
    const t = this.newTag().trim().toLowerCase().replaceAll(',', '');
    if (t.length === 0) return;
    this.tags.update(set => new Set(set).add(t));
    this.newTag.set('');
  }

  openAddForm(): void {
    this.newProductName.set(this.search().trim());
    this.showAddForm.set(true);
  }

  onIngredientCreated(ing: Ingredient): void {
    this.ingredients.update(list => [...list, ing].sort((a, b) => a.name.localeCompare(b.name, 'pl')));
    this.addItem(ing);
    this.showAddForm.set(false);
  }

  addItem(ing: Ingredient): void {
    this.items.update(rows => [...rows, { ing, grams: 100 }]);
    this.search.set('');
  }

  removeItem(index: number): void {
    this.items.update(rows => rows.filter((_, i) => i !== index));
  }

  setGrams(index: number, grams: number): void {
    this.items.update(rows => rows.map((r, i) => i === index ? { ...r, grams } : r));
  }

  private steps(): string[] {
    return this.stepsText()
      .split('\n')
      .map(s => s.trim())
      .filter(s => s.length > 0);
  }

  save(): void {
    if (!this.valid() || this.saving()) return;
    this.saving.set(true);
    this.error.set(null);
    this.api.createRecipe({
      name: this.name().trim(),
      timeMin: this.timeMin(),
      servings: this.servings(),
      tags: [...this.tags()],
      steps: this.steps(),
      items: this.items().map(r => ({ ingredientId: r.ing.id, grams: r.grams }))
    }).subscribe({
      next: res => this.router.navigate(['/recipe', res.id]),
      error: err => {
        this.saving.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się zapisać przepisu.'));
      }
    });
  }
}

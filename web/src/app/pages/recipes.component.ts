import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { PlanStateService } from '../core/plan-state.service';
import { apiErrorMessage } from '../core/api-error';
import { normalize } from '../core/ingredient-search';
import { RECIPE_CATEGORIES, RecipeCard } from '../core/models';

type SortKey = 'fav' | 'name' | 'cost' | 'time' | 'kcal';

const SORT_OPTIONS: { key: SortKey; label: string }[] = [
  { key: 'fav', label: '❤️ Ulubione najpierw' },
  { key: 'name', label: 'Nazwa A–Z' },
  { key: 'cost', label: 'Najtańsze' },
  { key: 'time', label: 'Najszybsze' },
  { key: 'kcal', label: 'Najmniej kcal' }
];

/** Wspólny katalog przepisów — seedy + przepisy dodane przez wszystkich użytkowników. */
@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Przepisy</h1>
        <a class="btn btn-primary btn-sm" routerLink="/add-recipe">➕ Dodaj przepis</a>
      </div>

      <p class="muted small">Wspólna baza — Twoje przepisy widzą wszyscy i trafiają do generatora jadłospisów.
        Ceny liczone dla: {{ store() }}, {{ people() }} os.</p>

      @if (error()) {
        <div class="notice error">{{ error() }}</div>
      } @else if (loading()) {
        <div class="card center"><div class="spinner"></div></div>
      } @else {
        <div class="card stack">
          <input type="text" placeholder="🔍 Szukaj przepisu…" [value]="query()"
                 (input)="query.set($any($event.target).value)" />
          <div class="row wrap" style="gap:6px">
            <span class="chip selectable" [class.on]="category() === null" (click)="category.set(null)">
              Wszystkie ({{ cards().length }})
            </span>
            @for (c of categories(); track c.name) {
              <span class="chip selectable" [class.on]="category() === c.name" (click)="category.set(c.name)">
                {{ c.name }} ({{ c.count }})
              </span>
            }
          </div>
          <div class="row wrap" style="gap:6px">
            @for (o of sortOptions; track o.key) {
              <span class="chip selectable" [class.on]="sort() === o.key" (click)="sort.set(o.key)">
                {{ o.label }}
              </span>
            }
          </div>
        </div>

        @if (visible().length === 0) {
          <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
            <p class="muted">Nic nie znaleziono. Zmień wyszukiwanie albo dodaj własny przepis.</p>
          </div>
        }

        @for (r of visible(); track r.recipeId) {
          <div class="card" style="cursor:pointer" (click)="open(r.recipeId)">
            <div class="row between" style="align-items:flex-start;gap:12px">
              <div style="flex:1">
                <div class="row" style="gap:8px;align-items:flex-start">
                  <button class="heart" [class.on]="r.favorite"
                          [attr.aria-label]="r.favorite ? 'Usuń z ulubionych' : 'Dodaj do ulubionych'"
                          (click)="toggleFav(r, $event)">
                    {{ r.favorite ? '❤️' : '🤍' }}
                  </button>
                  <div>
                    <div style="font-weight:800;font-size:16px">{{ r.name }}</div>
                    <div class="row wrap" style="gap:4px;margin-top:4px">
                      <span class="chip" style="padding:2px 8px">{{ r.category }}</span>
                      <span class="chip" style="padding:2px 8px">⏱ {{ r.timeMin }} min</span>
                      @for (t of r.tags; track t) { <span class="tag">{{ t }}</span> }
                      @if (r.hasPromo) { <span class="badge-promo">🏷️ promocja</span> }
                    </div>
                    <div class="muted small" style="margin-top:4px">
                      {{ r.kcal | number:'1.0-0' }} kcal · B {{ r.protein }} g · W {{ r.carbs }} g · T {{ r.fat }} g / porcja
                    </div>
                  </div>
                </div>
              </div>
              <div style="text-align:right">
                <div class="mono-price" style="font-size:18px;font-weight:800">{{ r.cost | number:'1.2-2' }} zł</div>
                @if (r.mine) {
                  <button class="btn btn-ghost btn-sm" style="margin-top:8px" (click)="remove(r, $event)">🗑 Usuń</button>
                }
              </div>
            </div>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .heart {
      background: none; border: none; font-size: 22px; line-height: 1;
      padding: 2px 4px; border-radius: 8px; transition: transform .1s;
    }
    .heart:hover { transform: scale(1.15); }
  `]
})
export class RecipesComponent {
  private api = inject(ApiService);
  private state = inject(PlanStateService);
  private router = inject(Router);

  readonly sortOptions = SORT_OPTIONS;

  store = signal(this.state.onboarding()?.store ?? 'Biedronka');
  people = signal(this.state.onboarding()?.people ?? 4);

  cards = signal<RecipeCard[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  query = signal('');
  sort = signal<SortKey>('fav');
  /** Aktywny filtr kategorii; null = wszystkie. */
  category = signal<string | null>(null);

  /** Kategorie obecne w danych, z licznikami — w kolejności ze wspólnej listy. */
  categories = computed(() => {
    const counts = new Map<string, number>();
    for (const c of this.cards()) counts.set(c.category, (counts.get(c.category) ?? 0) + 1);
    const order = [...RECIPE_CATEGORIES, 'Inne'];
    return order
      .filter(name => counts.has(name))
      .map(name => ({ name, count: counts.get(name)! }));
  });

  visible = computed(() => {
    const q = normalize(this.query().trim());
    const cat = this.category();
    const list = this.cards().filter(c =>
      (!cat || c.category === cat) && (!q || normalize(c.name).includes(q)));
    const byName = (a: RecipeCard, b: RecipeCard) => a.name.localeCompare(b.name, 'pl');
    switch (this.sort()) {
      case 'fav': return [...list].sort((a, b) => Number(b.favorite) - Number(a.favorite) || byName(a, b));
      case 'cost': return [...list].sort((a, b) => a.cost - b.cost || byName(a, b));
      case 'time': return [...list].sort((a, b) => a.timeMin - b.timeMin || byName(a, b));
      case 'kcal': return [...list].sort((a, b) => a.kcal - b.kcal || byName(a, b));
      default: return [...list].sort(byName);
    }
  });

  constructor() {
    this.api.recipes(this.store(), this.people()).subscribe({
      next: cards => { this.cards.set(cards); this.loading.set(false); },
      error: err => {
        this.loading.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się pobrać przepisów.'));
      }
    });
  }

  open(id: number): void {
    this.router.navigate(['/recipe', id]);
  }

  /** Optymistyczna zmiana serduszka — przy błędzie API cofamy. */
  toggleFav(card: RecipeCard, ev: Event): void {
    ev.stopPropagation();
    const next = !card.favorite;
    this.setFavorite(card.recipeId, next);
    const call = next ? this.api.addFavorite(card.recipeId) : this.api.removeFavorite(card.recipeId);
    call.subscribe({ error: () => this.setFavorite(card.recipeId, !next) });
  }

  remove(card: RecipeCard, ev: Event): void {
    ev.stopPropagation();
    if (!confirm(`Usunąć przepis „${card.name}"? Zniknie też u innych użytkowników.`)) return;
    this.api.deleteRecipe(card.recipeId).subscribe({
      next: () => this.cards.update(cs => cs.filter(c => c.recipeId !== card.recipeId)),
      error: err => this.error.set(apiErrorMessage(err, 'Nie udało się usunąć przepisu.'))
    });
  }

  private setFavorite(recipeId: number, favorite: boolean): void {
    this.cards.update(cs => cs.map(c => c.recipeId === recipeId ? { ...c, favorite } : c));
  }
}

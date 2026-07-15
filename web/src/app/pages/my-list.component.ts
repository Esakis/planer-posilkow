import { Component, OnDestroy, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { readJson, removeKey } from '../core/storage';
import { searchIngredients } from '../core/ingredient-search';
import {
  CompareResponse, Ingredient, IngredientLine, SavedList, SavedListItem, SavedListItemInput,
  ShoppingList, StoreName
} from '../core/models';
import { AddIngredientFormComponent } from '../shared/add-ingredient-form.component';
import { PriceLegendComponent } from '../shared/price-legend.component';
import { PriceEditorComponent } from '../shared/price-editor.component';

/** Klucz starej, pojedynczej listy z localStorage — migrowana jednorazowo do bazy. */
const LEGACY_KEY = 'tanitydzien.mylist.v1';

interface LegacyItem {
  id: number;
  name: string;
  aisle: string;
  grams: number;
}

const STORE_EMOJI: Record<StoreName, string> = {
  Biedronka: '🐞',
  Lidl: '🔵',
  Auchan: '🦜'
};

@Component({
  selector: 'app-my-list',
  standalone: true,
  imports: [RouterLink, DecimalPipe, AddIngredientFormComponent, PriceLegendComponent, PriceEditorComponent],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Moje listy zakupów</h1>
        <a class="btn btn-ghost btn-sm" routerLink="/">← Start</a>
      </div>

      <p class="muted small">Twoje listy są zapisane na koncie — odhaczaj kupione produkty w sklepie,
        a my porównamy koszt całej listy w Biedronce, Lidlu i Auchan.</p>

      @if (loadError()) {
        <div class="notice error">{{ loadError() }}</div>
      } @else if (listsLoading()) {
        <div class="card center"><div class="spinner"></div><p class="muted small" style="margin-top:10px">Wczytuję listy…</p></div>
      } @else {

      <div class="card stack">
        <div class="row" style="gap:6px;flex-wrap:wrap">
          @for (l of lists(); track l.id) {
            <span class="chip selectable" [class.on]="l.id === activeId()" (click)="switchList(l.id)">
              {{ l.name }}
            </span>
          }
          <span class="chip selectable" (click)="openNewListForm()">➕ Nowa lista</span>
        </div>

        @if (newListMode()) {
          <div class="row" style="gap:8px">
            <input type="text" maxlength="60" style="flex:1" placeholder="np. Zakupy na weekend"
                   [value]="newListName()" (input)="newListName.set($any($event.target).value)"
                   (keydown.enter)="createList()" />
            <button class="btn btn-primary btn-sm" [disabled]="creating()" (click)="createList()">Utwórz</button>
            <button class="btn btn-ghost btn-sm" (click)="newListMode.set(false)">Anuluj</button>
          </div>
        }
      </div>

      @if (active(); as list) {
        <div class="card stack">
          <div class="row between" style="gap:8px">
            @if (renameMode()) {
              <input type="text" maxlength="60" style="flex:1" [value]="renameName()"
                     (input)="renameName.set($any($event.target).value)"
                     (keydown.enter)="confirmRename()" />
              <button class="btn btn-primary btn-sm" (click)="confirmRename()">OK</button>
              <button class="btn btn-ghost btn-sm" (click)="renameMode.set(false)">Anuluj</button>
            } @else {
              <div style="flex:1">
                <div style="font-weight:800;font-size:17px">{{ list.name }}</div>
                @if (list.items.length > 0) {
                  <div class="muted small">kupione {{ boughtCount() }}/{{ list.items.length }}</div>
                }
              </div>
              <button class="btn btn-ghost btn-sm" (click)="openRename()">✏️ Nazwa</button>
              <button class="btn btn-ghost btn-sm" (click)="deleteList()">🗑 Usuń</button>
            }
          </div>

          @if (list.items.length === 0) {
            <div class="muted small center">Lista jest pusta — wyszukaj pierwszy produkt poniżej.</div>
          }
          @for (row of list.items; track row.ingredientId; let i = $index) {
            <div class="row between" style="gap:10px;border-top:1px solid var(--line);padding-top:8px">
              <input type="checkbox" style="width:20px;height:20px;align-self:center;accent-color:var(--green)"
                     [checked]="row.checked" (change)="toggleChecked(i)" />
              <div style="flex:1" [style.opacity]="row.checked ? 0.55 : 1">
                <div style="font-weight:700"
                     [style.text-decoration]="row.checked ? 'line-through' : 'none'">{{ row.name }}</div>
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
                <div class="muted small" style="margin-top:6px">Nie znamy tego produktu.</div>
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

          @if (saveError()) {
            <div class="notice error">{{ saveError() }}</div>
          }
        </div>

        <button class="btn btn-primary btn-block" [disabled]="list.items.length === 0 || comparing()"
                (click)="compare()">
          {{ comparing() ? 'Porównuję…' : '💰 Gdzie najtaniej?' }}
        </button>
      } @else {
        <div class="card center stack" style="padding-top:32px;padding-bottom:32px">
          <p class="muted">Nie masz jeszcze żadnej listy — utwórz pierwszą przyciskiem „➕ Nowa lista".</p>
        </div>
      }

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

          @if (d.verifiedComparison) {
            <p class="muted small center">
              ✅ Różnica policzona wyłącznie ze zweryfikowanych cen ({{ d.verifiedItems }} pozycji wspólnych).
            </p>
          } @else {
            <p class="muted small center">
              ⚠️ Różnica szacunkowa — popraw ceny w podglądzie listy, aby porównanie było wiarygodne.
            </p>
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
                  <div class="row between" style="gap:12px;padding:8px 0;border-top:1px solid var(--line)">
                    <div>
                      <div style="font-weight:700">{{ it.ingredient }}</div>
                      <div class="muted small">{{ it.displayQty }}
                        @if (it.onPromo && it.promoNote) { · <span style="color:var(--promo);font-weight:700">🏷️ {{ it.promoNote }}</span> }
                      </div>
                    </div>
                    <span class="mono-price" style="font-weight:700;white-space:nowrap"
                          [class.price-predicted]="it.source === 'predicted'"
                          [class.price-user]="it.source === 'user'"
                          [class.price-editable]="it.source !== 'verified'"
                          (click)="editPrice(it)">{{ it.cost | number:'1.2-2' }} zł</span>
                  </div>
                  @if (editingPrice() === it.ingredientId) {
                    <app-price-editor [ingredientId]="it.ingredientId" [ingredientName]="it.ingredient"
                                      [initialStore]="detailStore() ?? 'Biedronka'"
                                      (saved)="onPriceSaved()" (cancelled)="editingPrice.set(null)" />
                  }
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
export class MyListComponent implements OnDestroy {
  private api = inject(ApiService);

  ingredients = signal<Ingredient[]>([]);
  loadError = signal<string | null>(null);

  lists = signal<SavedList[]>([]);
  activeId = signal<number | null>(null);
  active = computed(() => this.lists().find(l => l.id === this.activeId()) ?? null);
  boughtCount = computed(() => (this.active()?.items ?? []).filter(i => i.checked).length);
  listsLoading = signal(true);

  newListMode = signal(false);
  newListName = signal('');
  creating = signal(false);
  renameMode = signal(false);
  renameName = signal('');
  saveError = signal<string | null>(null);

  search = signal('');
  showAddForm = signal(false);
  newProductName = signal('');

  comparing = signal(false);
  error = signal<string | null>(null);
  result = signal<CompareResponse | null>(null);

  listLoading = signal(false);
  detailStore = signal<StoreName | null>(null);
  detail = signal<ShoppingList | null>(null);
  /** Id składnika, którego cena jest właśnie edytowana w podglądzie koszyka. */
  editingPrice = signal<number | null>(null);

  /** Debounce autozapisu — każda zmiana listy trafia do bazy po chwili spokoju. */
  private saveTimer: ReturnType<typeof setTimeout> | null = null;

  matches = computed(() => {
    const used = new Set((this.active()?.items ?? []).map(r => r.ingredientId));
    return searchIngredients(this.ingredients(), this.search(), used);
  });

  constructor() {
    this.api.ingredients().subscribe({
      next: list => this.ingredients.set(list),
      error: err => this.loadError.set(apiErrorMessage(err, 'Nie udało się pobrać produktów. Sprawdź, czy API działa.'))
    });
    this.loadLists();
  }

  ngOnDestroy(): void {
    this.flushSave();
  }

  emoji(store: StoreName): string { return STORE_EMOJI[store] ?? '🛒'; }

  // ---------- listy ----------

  private loadLists(): void {
    this.api.savedLists().subscribe({
      next: lists => {
        this.lists.set(lists);
        if (lists.length === 0) {
          this.migrateLegacyList();
          return;
        }
        this.activeId.set(lists[0].id);
        this.listsLoading.set(false);
      },
      error: err => {
        this.listsLoading.set(false);
        this.loadError.set(apiErrorMessage(err, 'Nie udało się pobrać Twoich list.'));
      }
    });
  }

  /** Jednorazowo przenosi starą listę z localStorage do bazy. */
  private migrateLegacyList(): void {
    const legacy = readJson<LegacyItem[]>(LEGACY_KEY) ?? [];
    if (legacy.length === 0) {
      this.listsLoading.set(false);
      return;
    }
    const items: SavedListItemInput[] = legacy
      .filter(r => r.grams >= 1)
      .map(r => ({ ingredientId: r.id, grams: r.grams, checked: false }));
    this.api.createSavedList({ name: 'Moja lista', items }).subscribe({
      next: list => {
        removeKey(LEGACY_KEY);
        this.lists.set([list]);
        this.activeId.set(list.id);
        this.listsLoading.set(false);
      },
      // gdy migracja się nie uda (np. usunięty składnik), zostawiamy localStorage i zaczynamy od zera
      error: () => this.listsLoading.set(false)
    });
  }

  switchList(id: number): void {
    if (id === this.activeId()) return;
    this.flushSave();
    this.activeId.set(id);
    this.renameMode.set(false);
    this.search.set('');
    this.clearResults();
  }

  openNewListForm(): void {
    this.newListName.set('');
    this.newListMode.set(true);
  }

  createList(): void {
    const name = this.newListName().trim();
    if (!name || this.creating()) return;
    this.creating.set(true);
    this.api.createSavedList({ name }).subscribe({
      next: list => {
        this.creating.set(false);
        this.newListMode.set(false);
        this.flushSave();
        this.lists.update(ls => [...ls, list]);
        this.activeId.set(list.id);
        this.clearResults();
      },
      error: err => {
        this.creating.set(false);
        this.saveError.set(apiErrorMessage(err, 'Nie udało się utworzyć listy.'));
      }
    });
  }

  openRename(): void {
    this.renameName.set(this.active()?.name ?? '');
    this.renameMode.set(true);
  }

  confirmRename(): void {
    const name = this.renameName().trim();
    if (!name) return;
    this.renameMode.set(false);
    this.updateActive(l => ({ ...l, name }));
  }

  deleteList(): void {
    const list = this.active();
    if (!list || !confirm(`Usunąć listę „${list.name}"?`)) return;
    if (this.saveTimer) { clearTimeout(this.saveTimer); this.saveTimer = null; }
    this.api.deleteSavedList(list.id).subscribe({
      next: () => {
        this.lists.update(ls => ls.filter(l => l.id !== list.id));
        this.activeId.set(this.lists()[0]?.id ?? null);
        this.clearResults();
      },
      error: err => this.saveError.set(apiErrorMessage(err, 'Nie udało się usunąć listy.'))
    });
  }

  // ---------- pozycje ----------

  addItem(ing: Ingredient): void {
    this.updateActive(l => ({
      ...l,
      items: [...l.items, { ingredientId: ing.id, name: ing.name, aisle: ing.aisle, grams: 500, checked: false }]
    }));
    this.search.set('');
    this.clearResults();
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

  removeItem(index: number): void {
    this.updateActive(l => ({ ...l, items: l.items.filter((_, i) => i !== index) }));
    this.clearResults();
  }

  setGrams(index: number, grams: number): void {
    this.updateActive(l => ({ ...l, items: l.items.map((r, i) => i === index ? { ...r, grams } : r) }));
    this.clearResults();
  }

  /** Odhaczenie nie unieważnia porównania cen — lista kosztuje tyle samo. */
  toggleChecked(index: number): void {
    this.updateActive(l => ({ ...l, items: l.items.map((r, i) => i === index ? { ...r, checked: !r.checked } : r) }));
  }

  // ---------- porównanie / koszyk ----------

  compare(): void {
    const items = this.pricedItems();
    if (items.length === 0 || this.comparing()) return;
    this.comparing.set(true);
    this.error.set(null);
    this.api.customCompare({ items }).subscribe({
      next: d => { this.result.set(d); this.comparing.set(false); },
      error: err => {
        this.comparing.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się porównać sklepów.'));
      }
    });
  }

  showList(store: StoreName): void {
    const items = this.pricedItems();
    if (items.length === 0 || this.listLoading()) return;
    this.listLoading.set(true);
    this.detailStore.set(store);
    this.error.set(null);
    this.api.customList({ items, store }).subscribe({
      next: l => { this.detail.set(l); this.listLoading.set(false); },
      error: err => {
        this.listLoading.set(false);
        this.detailStore.set(null);
        this.error.set(apiErrorMessage(err, 'Nie udało się pobrać listy.'));
      }
    });
  }

  private pricedItems(): { ingredientId: number; grams: number }[] {
    return (this.active()?.items ?? [])
      .filter(r => r.grams >= 1)
      .map(r => ({ ingredientId: r.ingredientId, grams: r.grams }));
  }

  /** Klik w szarą/niebieską cenę otwiera edytor; gazetkowej (czarnej) nie edytujemy. */
  editPrice(it: IngredientLine): void {
    if (it.source === 'verified') return;
    this.editingPrice.set(this.editingPrice() === it.ingredientId ? null : it.ingredientId);
  }

  /** Po zapisie ceny odświeżamy podgląd koszyka i (jeśli otwarte) porównanie sklepów. */
  onPriceSaved(): void {
    this.editingPrice.set(null);
    const store = this.detailStore();
    if (store) this.showList(store);
    if (this.result()) this.compare();
  }

  // ---------- autozapis ----------

  /** Zmienia aktywną listę lokalnie i planuje zapis do bazy. */
  private updateActive(fn: (list: SavedList) => SavedList): void {
    const id = this.activeId();
    if (id === null) return;
    this.lists.update(ls => ls.map(l => l.id === id ? fn(l) : l));
    this.scheduleSave();
  }

  private scheduleSave(): void {
    if (this.saveTimer) clearTimeout(this.saveTimer);
    this.saveTimer = setTimeout(() => { this.saveTimer = null; this.doSave(); }, 800);
  }

  /** Natychmiast zapisuje odłożone zmiany (przełączenie listy, wyjście ze strony). */
  private flushSave(): void {
    if (!this.saveTimer) return;
    clearTimeout(this.saveTimer);
    this.saveTimer = null;
    this.doSave();
  }

  private doSave(): void {
    const list = this.active();
    if (!list) return;
    this.saveError.set(null);
    this.api.updateSavedList(list.id, {
      name: list.name,
      items: list.items
        .filter(r => r.grams >= 1)
        .map(r => ({ ingredientId: r.ingredientId, grams: r.grams, checked: r.checked }))
    }).subscribe({
      // odpowiedzi nie wgrywamy do stanu — lokalne zmiany mogły już pójść dalej
      error: err => this.saveError.set(apiErrorMessage(err, 'Nie udało się zapisać zmian na koncie.'))
    });
  }

  private clearResults(): void {
    this.result.set(null);
    this.detail.set(null);
    this.detailStore.set(null);
  }
}

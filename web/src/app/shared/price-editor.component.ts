import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { StoreName } from '../core/models';

const STORES: StoreName[] = ['Biedronka', 'Lidl', 'Auchan'];

/** Formularz poprawienia ceny produktu: sklep + cena opakowania. Zapis obowiązuje wszystkich. */
@Component({
  selector: 'app-price-editor',
  standalone: true,
  template: `
    <div class="card stack" style="border-color:var(--green);--space:10px">
      <div style="font-weight:700">Popraw cenę: {{ ingredientName }}</div>
      <div class="row wrap" style="gap:6px">
        @for (s of stores; track s) {
          <span class="chip selectable" [class.on]="store() === s" (click)="store.set(s)">{{ s }}</span>
        }
      </div>
      <div class="row" style="gap:8px">
        <input type="number" min="0.01" step="0.01" style="width:120px" placeholder="np. 4,99"
               [value]="price() ?? ''" (input)="price.set(+$any($event.target).value || null)"
               (keydown.enter)="save()" />
        <span class="muted small">zł za opakowanie</span>
      </div>
      @if (error()) {
        <div class="notice error">{{ error() }}</div>
      }
      <div class="row" style="gap:8px">
        <button class="btn btn-primary btn-sm" [disabled]="busy()" (click)="save()">
          {{ busy() ? 'Zapisywanie…' : 'Zapisz cenę' }}
        </button>
        <button class="btn btn-ghost btn-sm" (click)="cancelled.emit()">Anuluj</button>
      </div>
    </div>
  `
})
export class PriceEditorComponent {
  private api = inject(ApiService);

  @Input({ required: true }) ingredientId!: number;
  @Input() ingredientName = '';
  @Input() set initialStore(v: StoreName) { this.store.set(v); }
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  readonly stores = STORES;
  store = signal<StoreName>('Biedronka');
  price = signal<number | null>(null);
  busy = signal(false);
  error = signal<string | null>(null);

  save(): void {
    const price = this.price();
    if (!price || price <= 0) {
      this.error.set('Podaj cenę większą od zera.');
      return;
    }
    if (this.busy()) return;
    this.busy.set(true);
    this.error.set(null);
    this.api.updatePrice(this.ingredientId, { store: this.store(), basePrice: price }).subscribe({
      next: () => { this.busy.set(false); this.saved.emit(); },
      error: err => {
        this.busy.set(false);
        this.error.set(apiErrorMessage(err, 'Nie udało się zapisać ceny.'));
      }
    });
  }
}

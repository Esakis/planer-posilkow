import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../core/api.service';
import { HistoryService } from '../core/history.service';
import { PlanStateService } from '../core/plan-state.service';
import { OnboardingRequest, StoreName } from '../core/models';

interface ExclusionOption { tag: string; label: string; }

@Component({
  selector: 'app-onboarding',
  standalone: true,
  template: `
    <div class="container stack">
      <div class="center stack">
        <h1 style="font-size:26px">Jadłospis na tydzień pod promocje</h1>
        <p class="muted">Powiedz nam kilka rzeczy — ułożymy obiady i listę zakupów
          dopasowane do tego, co jest teraz tanie w Twoim sklepie.</p>
      </div>

      <div class="card">
        <label class="field">
          <span class="lbl">Sklep</span>
          <div class="segmented">
            @for (s of stores; track s) {
              <div class="seg" [class.on]="store() === s" (click)="store.set(s)">{{ s }}</div>
            }
          </div>
        </label>

        <label class="field">
          <span class="lbl">Ile osób jada?</span>
          <div class="stepper">
            <button type="button" (click)="bump('people', -1)">−</button>
            <span class="val">{{ people() }}</span>
            <button type="button" (click)="bump('people', 1)">+</button>
          </div>
        </label>

        <label class="field">
          <span class="lbl">Ile obiadów gotujesz w tygodniu?</span>
          <div class="stepper">
            <button type="button" (click)="bump('dinners', -1)">−</button>
            <span class="val">{{ dinners() }}</span>
            <button type="button" (click)="bump('dinners', 1)">+</button>
          </div>
        </label>

        <label class="field" for="budget">
          <span class="lbl">Budżet na tydzień
            <span class="hint">— opcjonalnie, ostrzeżemy, gdy plan go przekroczy</span>
          </span>
          <input id="budget" type="number" min="0" step="10" [value]="budget()"
                 (input)="setBudget(+$any($event.target).value)" placeholder="np. 150" />
        </label>

        <label class="field">
          <span class="lbl">Czego nie jecie?
            <span class="hint">— kliknij, żeby wykluczyć</span>
          </span>
          <div class="row wrap">
            @for (o of exclusionOptions; track o.tag) {
              <span class="chip selectable" [class.on]="isExcluded(o.tag)"
                    (click)="toggle(o.tag)">{{ o.label }}</span>
            }
          </div>
        </label>

        <label class="field" for="minProtein">
          <span class="lbl">Min. białka na porcję
            <span class="hint">— {{ minProtein() > 0 ? minProtein() + ' g' : 'bez limitu' }}</span>
          </span>
          <input id="minProtein" type="range" min="0" max="50" step="5"
                 [value]="minProtein()" (input)="minProtein.set(+$any($event.target).value)" />
        </label>

        <label class="field" for="maxKcal">
          <span class="lbl">Maks. kcal na porcję
            <span class="hint">— {{ maxKcal() > 0 ? maxKcal() + ' kcal' : 'bez limitu' }}</span>
          </span>
          <input id="maxKcal" type="range" min="0" max="900" step="50"
                 [value]="maxKcal()" (input)="maxKcal.set(+$any($event.target).value)" />
        </label>
      </div>

      @if (error()) { <div class="notice error">{{ error() }}</div> }

      <button class="btn btn-primary btn-block" [disabled]="loading()" (click)="generate()">
        @if (loading()) { <span class="spinner" style="width:18px;height:18px;border-width:2px"></span> Układam… }
        @else { Ułóż jadłospis → }
      </button>
    </div>
  `
})
export class OnboardingComponent {
  private api = inject(ApiService);
  private state = inject(PlanStateService);
  private history = inject(HistoryService);
  private router = inject(Router);

  readonly stores: StoreName[] = ['Biedronka', 'Lidl', 'Auchan'];
  readonly exclusionOptions: ExclusionOption[] = [
    { tag: 'mięso', label: 'Wege (bez mięsa)' },
    { tag: 'wieprzowina', label: 'Bez wieprzowiny' },
    { tag: 'ryby', label: 'Bez ryb' }
  ];

  store = signal<StoreName>('Biedronka');
  people = signal(4);
  dinners = signal(5);
  budget = signal<number>(150);
  exclusions = signal<string[]>([]);
  minProtein = signal(0);  // 0 = bez limitu
  maxKcal = signal(0);     // 0 = bez limitu

  loading = signal(false);
  error = signal<string | null>(null);

  setBudget(value: number): void {
    // ujemne i niesparsowane (NaN) traktujemy jako brak budżetu
    this.budget.set(Number.isFinite(value) && value > 0 ? value : 0);
  }

  bump(which: 'people' | 'dinners', delta: number): void {
    if (which === 'people') this.people.set(clamp(this.people() + delta, 1, 12));
    else this.dinners.set(clamp(this.dinners() + delta, 1, 7));
  }

  isExcluded(tag: string): boolean {
    return this.exclusions().includes(tag);
  }

  toggle(tag: string): void {
    this.exclusions.update(list =>
      list.includes(tag) ? list.filter(t => t !== tag) : [...list, tag]);
  }

  generate(): void {
    this.error.set(null);
    this.loading.set(true);

    const req: OnboardingRequest = {
      people: this.people(),
      weeklyBudget: this.budget() || 0,
      store: this.store(),
      exclusions: this.exclusions(),
      dinners: this.dinners(),
      minProteinPerServing: this.minProtein() > 0 ? this.minProtein() : null,
      maxKcalPerServing: this.maxKcal() > 0 ? this.maxKcal() : null
    };

    this.api.generate(req).subscribe({
      next: menu => {
        this.state.set(req, menu);
        this.history.add(req, menu);
        this.loading.set(false);
        this.router.navigate(['/menu']);
      },
      error: err => {
        this.loading.set(false);
        this.error.set(err?.error?.message ??
          'Nie udało się połączyć z API. Sprawdź, czy backend działa na :5080.');
      }
    });
  }
}

function clamp(v: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, v));
}

import { Component, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { HistoryService } from '../core/history.service';
import { PlanStateService } from '../core/plan-state.service';
import { MacroFilters, OnboardingRequest, StoreName } from '../core/models';

interface ExclusionOption { tag: string; label: string; }

type MacroKey = 'protein' | 'carbs' | 'fat' | 'kcal';
type PresetKey = 'none' | 'protein' | 'lowcarb' | 'light';

interface MacroDef {
  key: MacroKey;
  label: string;
  unit: string;
  floor: number;
  ceil: number;
  step: number;
}

/** Suwaki wg sekcji 3a planu; min na dole zakresu / max na górze = bez limitu. */
const MACRO_DEFS: MacroDef[] = [
  { key: 'protein', label: 'Białko', unit: 'g', floor: 0, ceil: 60, step: 5 },
  { key: 'carbs', label: 'Węglowodany', unit: 'g', floor: 0, ceil: 120, step: 5 },
  { key: 'fat', label: 'Tłuszcze', unit: 'g', floor: 0, ceil: 50, step: 5 },
  { key: 'kcal', label: 'Kalorie', unit: 'kcal', floor: 200, ceil: 1200, step: 50 }
];

type MacroState = Record<MacroKey, { min: number; max: number }>;

function fullRange(): MacroState {
  return {
    protein: { min: 0, max: 60 },
    carbs: { min: 0, max: 120 },
    fat: { min: 0, max: 50 },
    kcal: { min: 200, max: 1200 }
  };
}

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

        <label class="field">
          <span class="lbl">Makro na porcję
            <span class="hint">— wybierz preset albo dostosuj suwakami</span>
          </span>
          <div class="row wrap">
            @for (p of presets; track p.key) {
              <span class="chip selectable" [class.on]="preset() === p.key"
                    (click)="applyPreset(p.key)">{{ p.label }}</span>
            }
          </div>
        </label>

        @if (preset() !== 'none' || showSliders()) {
          <button type="button" class="btn btn-ghost btn-sm" style="align-self:flex-start"
                  (click)="showSliders.set(!showSliders())">
            {{ showSliders() ? '▴ Zwiń suwaki' : '▾ Dostosuj suwakami' }}
          </button>
        } @else {
          <button type="button" class="btn btn-ghost btn-sm" style="align-self:flex-start"
                  (click)="showSliders.set(true)">▾ Dostosuj suwakami</button>
        }

        @if (showSliders()) {
          @for (d of macroDefs; track d.key) {
            <label class="field">
              <span class="lbl">{{ d.label }}
                <span class="hint">— {{ rangeLabel(d) }}</span>
              </span>
              <div class="row" style="gap:10px;align-items:center">
                <input type="range" style="flex:1" [min]="d.floor" [max]="d.ceil" [step]="d.step"
                       [value]="macro()[d.key].min" (input)="setMacro(d.key, 'min', +$any($event.target).value)" />
                <input type="range" style="flex:1" [min]="d.floor" [max]="d.ceil" [step]="d.step"
                       [value]="macro()[d.key].max" (input)="setMacro(d.key, 'max', +$any($event.target).value)" />
              </div>
            </label>
          }
        }

        @if (poolCount() !== null) {
          <div class="notice" [class.warn]="poolCount()! < dinners()"
               style="padding:8px 12px">
            @if (poolCount()! >= dinners()) {
              🍲 <b>{{ poolCount() }}</b> {{ recipesWord(poolCount()!) }} pasuje do tych ustawień
            } @else {
              ⚠️ Tylko <b>{{ poolCount() }}</b> {{ recipesWord(poolCount()!) }} pasuje — poluzuj filtry,
              żeby ułożyć {{ dinners() }} obiadów
            }
          </div>
        }
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

  readonly macroDefs = MACRO_DEFS;
  readonly presets: { key: PresetKey; label: string }[] = [
    { key: 'none', label: 'Bez filtra' },
    { key: 'protein', label: '💪 Więcej białka' },
    { key: 'lowcarb', label: '🥦 Mniej węgli' },
    { key: 'light', label: '🍃 Lekkie' }
  ];

  store = signal<StoreName>('Biedronka');
  people = signal(4);
  dinners = signal(5);
  budget = signal<number>(150);
  exclusions = signal<string[]>([]);

  macro = signal<MacroState>(fullRange());
  preset = signal<PresetKey>('none');
  showSliders = signal(false);
  poolCount = signal<number | null>(null);

  loading = signal(false);
  error = signal<string | null>(null);

  private countDebounce: ReturnType<typeof setTimeout> | null = null;

  constructor() {
    // licznik „ile przepisów pasuje" — na żywo, z debounce, przy każdej zmianie filtrów
    effect(() => {
      const body = { exclusions: this.exclusions(), macro: this.toFilters() };
      if (this.countDebounce) clearTimeout(this.countDebounce);
      this.countDebounce = setTimeout(() => {
        this.api.poolCount(body).subscribe({
          next: r => this.poolCount.set(r.count),
          error: () => this.poolCount.set(null) // licznik to bonus — brak API nie blokuje formularza
        });
      }, 300);
    });
  }

  setBudget(value: number): void {
    // ujemne i niesparsowane (NaN) traktujemy jako brak budżetu
    this.budget.set(Number.isFinite(value) && value > 0 ? value : 0);
  }

  applyPreset(key: PresetKey): void {
    this.preset.set(key);
    const state = fullRange();
    if (key === 'protein') state.protein.min = 30;
    if (key === 'lowcarb') state.carbs.max = 40;
    if (key === 'light') state.kcal.max = 500;
    this.macro.set(state);
  }

  setMacro(key: MacroKey, which: 'min' | 'max', value: number): void {
    this.preset.set('none');
    this.macro.update(state => {
      const range = { ...state[key], [which]: value };
      // uchwyty nie mogą się minąć
      if (range.min > range.max) {
        if (which === 'min') range.max = range.min;
        else range.min = range.max;
      }
      return { ...state, [key]: range };
    });
  }

  rangeLabel(d: MacroDef): string {
    const { min, max } = this.macro()[d.key];
    if (min <= d.floor && max >= d.ceil) return 'bez limitu';
    const lo = min > d.floor ? `od ${min} ${d.unit}` : '';
    const hi = max < d.ceil ? `do ${max} ${d.unit}` : '';
    return [lo, hi].filter(Boolean).join(' ');
  }

  recipesWord(n: number): string {
    if (n === 1) return 'przepis';
    const tens = n % 100;
    const ones = n % 10;
    if (ones >= 2 && ones <= 4 && (tens < 12 || tens > 14)) return 'przepisy';
    return 'przepisów';
  }

  /** Zamienia stan suwaków na filtry API (pełny zakres = null = bez limitu). */
  private toFilters(): MacroFilters | null {
    const m = this.macro();
    const f: MacroFilters = {
      minProtein: m.protein.min > 0 ? m.protein.min : null,
      maxProtein: m.protein.max < 60 ? m.protein.max : null,
      minCarbs: m.carbs.min > 0 ? m.carbs.min : null,
      maxCarbs: m.carbs.max < 120 ? m.carbs.max : null,
      minFat: m.fat.min > 0 ? m.fat.min : null,
      maxFat: m.fat.max < 50 ? m.fat.max : null,
      minKcal: m.kcal.min > 200 ? m.kcal.min : null,
      maxKcal: m.kcal.max < 1200 ? m.kcal.max : null
    };
    return Object.values(f).some(v => v !== null) ? f : null;
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
      weeklyBudget: this.budget(),
      store: this.store(),
      exclusions: this.exclusions(),
      dinners: this.dinners(),
      macro: this.toFilters()
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
        this.error.set(apiErrorMessage(err,
          'Nie udało się połączyć z API. Sprawdź, czy backend działa na :5080.'));
      }
    });
  }
}

function clamp(v: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, v));
}

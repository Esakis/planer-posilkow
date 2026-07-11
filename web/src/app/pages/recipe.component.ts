import { Component, effect, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location, DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { PlanStateService } from '../core/plan-state.service';
import { RecipeDetail } from '../core/models';

@Component({
  selector: 'app-recipe',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <div class="container stack" [class.cooking]="cookMode()">
      <button class="btn btn-ghost btn-sm" (click)="back()" style="align-self:flex-start">← Wróć</button>

      @if (loading()) {
        <div class="card center"><div class="spinner"></div></div>
      } @else if (error()) {
        <div class="notice error">{{ error() }}</div>
      } @else {
        @if (recipe(); as r) {

        <div class="stack">
          <h1 style="font-size:24px">{{ r.name }}</h1>
          <div class="row wrap" style="gap:6px">
            <span class="chip">⏱ {{ r.timeMin }} min</span>
            <span class="chip">🍽 {{ r.servings }} porcji</span>
            @for (t of r.tags; track t) { <span class="tag">{{ t }}</span> }
          </div>
          <div class="row" style="gap:16px;flex-wrap:wrap">
            <span class="muted small"><b>{{ r.macroPerServing.kcal | number }}</b> kcal</span>
            <span class="muted small">białko <b>{{ r.macroPerServing.protein }} g</b></span>
            <span class="muted small">węgle <b>{{ r.macroPerServing.carbs }} g</b></span>
            <span class="muted small">tłuszcze <b>{{ r.macroPerServing.fat }} g</b></span>
            <span class="muted small">/ porcja</span>
          </div>
        </div>

        <div class="card">
          <h2 style="font-size:16px;margin-bottom:10px">Składniki <span class="muted small">({{ r.servings }} porcji)</span></h2>
          <div>
            @for (ing of r.ingredients; track ing.name) {
              <div class="row between" style="padding:8px 0;border-top:1px solid var(--line)">
                <span style="font-weight:600">{{ ing.name }}</span>
                <span class="muted">{{ ing.displayQty }}</span>
              </div>
            }
          </div>
        </div>

        <div class="card">
          <div class="row between" style="margin-bottom:12px">
            <h2 style="font-size:16px">Przygotowanie</h2>
            <button class="btn btn-ghost btn-sm" (click)="cookMode.set(!cookMode())">
              {{ cookMode() ? '✕ Zwykły widok' : '👨‍🍳 Tryb gotowania' }}
            </button>
          </div>
          <ol class="steps">
            @for (step of r.steps; track $index) {
              <li>{{ stripNumber(step) }}</li>
            }
          </ol>
        </div>
        }
      }
    </div>
  `,
  styles: [`
    ol.steps { margin: 0; padding-left: 22px; }
    ol.steps li { padding: 8px 0; line-height: 1.55; }
    .cooking ol.steps li { font-size: 20px; padding: 14px 0; }
    .cooking h1 { font-size: 28px; }
  `]
})
export class RecipeComponent {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private location = inject(Location);
  private state = inject(PlanStateService);

  recipe = signal<RecipeDetail | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  cookMode = signal(false);

  private wakeLock: WakeLockSentinel | null = null;
  /** Czy blokada powinna być trzymana — steruje przypisaniem po async request. */
  private wakeLockWanted = false;

  constructor() {
    // wybudź ekran w trybie gotowania (Wake Lock — best effort);
    // przeglądarka zwalnia blokadę przy ukryciu karty, więc odnawiamy ją po powrocie
    effect(onCleanup => {
      if (!this.cookMode()) return;
      this.wakeLockWanted = true;
      this.acquireWakeLock();
      const onVisible = () => {
        if (document.visibilityState === 'visible') this.acquireWakeLock();
      };
      document.addEventListener('visibilitychange', onVisible);
      onCleanup(() => {
        // wykonuje się przy wyłączeniu trybu gotowania i przy niszczeniu komponentu
        document.removeEventListener('visibilitychange', onVisible);
        this.wakeLockWanted = false;
        this.releaseWakeLock();
      });
    });

    const id = Number(this.route.snapshot.paramMap.get('id'));
    const people = this.state.onboarding()?.people ?? 4;

    if (!id) {
      this.loading.set(false);
      this.error.set('Nieprawidłowy przepis.');
      return;
    }

    this.api.recipe(id, people).subscribe({
      next: r => { this.recipe.set(r); this.loading.set(false); },
      error: () => {
        this.loading.set(false);
        this.error.set('Nie udało się wczytać przepisu.');
      }
    });
  }

  private async acquireWakeLock(): Promise<void> {
    try {
      const lock = await navigator.wakeLock?.request('screen') ?? null;
      // tryb gotowania mógł zostać wyłączony w trakcie oczekiwania — wtedy zwalniamy od razu
      if (this.wakeLockWanted) {
        this.wakeLock?.release().catch(() => {});
        this.wakeLock = lock;
      } else {
        lock?.release().catch(() => {});
      }
    } catch {
      // brak wsparcia / odmowa — tryb gotowania działa dalej
    }
  }

  private releaseWakeLock(): void {
    this.wakeLock?.release().catch(() => {});
    this.wakeLock = null;
  }

  back(): void { this.location.back(); }

  // kroki w bazie mają prefiks "1. " — usuwamy, bo <ol> numeruje sam
  stripNumber(step: string): string {
    return step.replace(/^\s*\d+\.\s*/, '');
  }
}

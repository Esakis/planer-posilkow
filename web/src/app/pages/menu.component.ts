import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { apiErrorMessage } from '../core/api-error';
import { PlanStateService } from '../core/plan-state.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  template: `
    @if (menu(); as m) {
      <div class="container stack">

        <div class="row between wrap">
          <div>
            <h1 style="font-size:22px">Twój jadłospis</h1>
            <p class="muted small">{{ m.week }} · {{ m.store }} · {{ m.people }} os. · {{ m.dinners }} obiadów</p>
          </div>
        </div>

        <!-- Podsumowanie oszczędności -->
        <div class="card" style="background:var(--green-soft);border-color:#bfe0cc">
          <div class="row between wrap" style="gap:16px">
            <div>
              <div class="muted small">Szacowany koszt tygodnia</div>
              <div style="font-size:28px;font-weight:800" class="mono-price">{{ m.estimatedCost | number:'1.2-2' }} zł</div>
            </div>
            @if (m.savings > 0) {
              <div style="text-align:right">
                <div class="muted small">Bez promocji zapłaciłbyś</div>
                <div class="mono-price" style="text-decoration:line-through;color:var(--muted)">{{ m.baselineCost | number:'1.2-2' }} zł</div>
                <div style="color:var(--green-dark);font-weight:800" class="mono-price">oszczędzasz {{ m.savings | number:'1.2-2' }} zł</div>
              </div>
            }
          </div>
        </div>

        @if (m.budgetNote) {
          <div class="notice warn">⚠️ {{ m.budgetNote }}</div>
        }

        <!-- Makro średnio dziennie -->
        <div class="card">
          <div class="muted small" style="margin-bottom:10px">Średnio na porcję dziennie</div>
          <div class="row between" style="text-align:center">
            <div style="flex:1"><div style="font-weight:800;font-size:18px">{{ m.perDayAvg.kcal | number:'1.0-0' }}</div><div class="muted small">kcal</div></div>
            <div style="flex:1"><div style="font-weight:800;font-size:18px">{{ m.perDayAvg.protein | number:'1.0-1' }} g</div><div class="muted small">białko</div></div>
            <div style="flex:1"><div style="font-weight:800;font-size:18px">{{ m.perDayAvg.carbs | number:'1.0-1' }} g</div><div class="muted small">węgle</div></div>
            <div style="flex:1"><div style="font-weight:800;font-size:18px">{{ m.perDayAvg.fat | number:'1.0-1' }} g</div><div class="muted small">tłuszcze</div></div>
          </div>
        </div>

        <!-- Dania -->
        <div class="stack">
          @for (d of m.dishes; track d.recipeId; let i = $index) {
            <div class="card">
              <div class="row between" style="align-items:flex-start;gap:12px">
                <div style="flex:1">
                  <div class="row wrap" style="gap:6px;margin-bottom:6px">
                    <span class="tag">Dzień {{ d.day }}</span>
                    @if (d.hasPromo) { <span class="badge-promo">🏷️ promocja</span> }
                  </div>
                  <a [routerLink]="['/recipe', d.recipeId]"
                     style="font-size:18px;font-weight:800;color:var(--ink);text-decoration:none">
                    {{ d.name }}
                  </a>
                  <div class="muted small" style="margin-top:4px">
                    ⏱ {{ d.timeMin }} min · {{ d.kcal | number:'1.0-0' }} kcal · B {{ d.protein | number:'1.0-0' }} / W {{ d.carbs | number:'1.0-0' }} / T {{ d.fat | number:'1.0-0' }}
                  </div>
                </div>
                <div style="text-align:right">
                  <div class="mono-price" style="font-size:18px;font-weight:800">{{ d.cost | number:'1.2-2' }} zł</div>
                  <button class="btn btn-ghost btn-sm" style="margin-top:8px"
                          [disabled]="swapping() === i" (click)="swap(i)">
                    @if (swapping() === i) { … } @else { ↻ Wymień }
                  </button>
                </div>
              </div>
            </div>
          }
        </div>

        @if (error()) { <div class="notice error">{{ error() }}</div> }

        <button class="btn btn-primary btn-block" routerLink="/compare">
          ⚖️ Porównaj sklepy — gdzie kupić taniej?
        </button>
        <button class="btn btn-ghost btn-block" (click)="goShopping()">
          🛒 Lista zakupów ({{ m.store }})
        </button>
        <button class="btn btn-ghost btn-block" (click)="startOver()">Zacznij od nowa</button>
      </div>
    } @else {
      <div class="container center stack" style="padding-top:60px">
        <p class="muted">Brak jadłospisu. Zacznij od ustawień.</p>
        <button class="btn btn-primary" routerLink="/">Ułóż jadłospis</button>
      </div>
    }
  `
})
export class MenuComponent {
  private api = inject(ApiService);
  private state = inject(PlanStateService);
  private router = inject(Router);

  menu = this.state.menu;
  swapping = signal<number | null>(null);
  error = signal<string | null>(null);

  swap(index: number): void {
    const ob = this.state.onboarding();
    const m = this.menu();
    if (!ob || !m) return;

    this.error.set(null);
    this.swapping.set(index);

    this.api.swap({
      recipeIds: m.dishes.map(d => d.recipeId),
      swapIndex: index,
      people: ob.people,
      weeklyBudget: ob.weeklyBudget,
      store: ob.store,
      exclusions: ob.exclusions,
      minProteinPerServing: ob.minProteinPerServing ?? null,
      maxKcalPerServing: ob.maxKcalPerServing ?? null
    }).subscribe({
      next: updated => {
        this.state.updateMenu(updated);
        this.swapping.set(null);
      },
      error: err => {
        this.swapping.set(null);
        this.error.set(apiErrorMessage(err, 'Nie udało się wymienić dania.'));
      }
    });
  }

  goShopping(): void {
    this.router.navigate(['/shopping']);
  }

  startOver(): void {
    this.state.clear();
    this.router.navigate(['/']);
  }
}

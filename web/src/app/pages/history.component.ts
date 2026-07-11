import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { HistoryEntry, HistoryService } from '../core/history.service';
import { PlanStateService } from '../core/plan-state.service';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [RouterLink, DatePipe, DecimalPipe],
  template: `
    <div class="container stack">
      <div class="row between">
        <h1 style="font-size:22px">Historia jadłospisów</h1>
        <a class="btn btn-ghost btn-sm" routerLink="/">← Nowy plan</a>
      </div>

      @if (entries().length === 0) {
        <div class="card center stack" style="padding-top:40px;padding-bottom:40px">
          <p class="muted">Nie masz jeszcze zapisanych jadłospisów.</p>
          <button class="btn btn-primary" routerLink="/">Ułóż pierwszy plan</button>
        </div>
      } @else {
        <p class="muted small">Jadłospisy zapisane na tym urządzeniu ({{ entries().length }}).
          Kliknij, żeby wrócić do planu.</p>

        @for (e of entries(); track e.id) {
          <div class="card">
            <div class="row between" style="align-items:flex-start;gap:12px">
              <div style="flex:1;cursor:pointer" (click)="restore(e)">
                <div class="muted small">{{ e.savedAt | date:'d.MM.y, HH:mm' }}</div>
                <div style="font-weight:800;font-size:17px;margin-top:2px">
                  {{ e.menu.store }} · {{ e.menu.dinners }} obiadów · {{ e.menu.people }} os.
                </div>
                <div class="muted small" style="margin-top:4px">
                  {{ dishNames(e) }}
                </div>
              </div>
              <div style="text-align:right">
                <div class="mono-price" style="font-size:18px;font-weight:800">
                  {{ e.menu.estimatedCost | number:'1.2-2' }} zł
                </div>
                <button class="btn btn-ghost btn-sm" style="margin-top:8px" (click)="remove(e.id)">Usuń</button>
              </div>
            </div>
          </div>
        }

        <button class="btn btn-ghost btn-block" (click)="clearAll()">Wyczyść historię</button>
      }
    </div>
  `
})
export class HistoryComponent {
  private history = inject(HistoryService);
  private state = inject(PlanStateService);
  private router = inject(Router);

  entries = this.history.entries;

  restore(e: HistoryEntry): void {
    this.state.set(e.onboarding, e.menu);
    this.router.navigate(['/menu']);
  }

  remove(id: number): void {
    this.history.remove(id);
  }

  clearAll(): void {
    this.history.clear();
  }

  dishNames(e: HistoryEntry): string {
    const names = e.menu.dishes.map(d => d.name);
    return names.slice(0, 3).join(', ') + (names.length > 3 ? '…' : '');
  }
}

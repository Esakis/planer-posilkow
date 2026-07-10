import { Injectable, computed, signal } from '@angular/core';
import { MenuResponse, OnboardingRequest, StoreName } from './models';

/**
 * Trzyma bieżący jadłospis i ustawienia onboardingu między ekranami
 * (jadłospis → lista zakupów → przepis). Bez tego przeładowanie/nawigacja
 * gubiłyby wygenerowany tydzień.
 */
@Injectable({ providedIn: 'root' })
export class PlanStateService {
  readonly onboarding = signal<OnboardingRequest | null>(null);
  readonly menu = signal<MenuResponse | null>(null);

  readonly hasMenu = computed(() => this.menu() !== null);

  set(onboarding: OnboardingRequest, menu: MenuResponse): void {
    this.onboarding.set(onboarding);
    this.menu.set(menu);
  }

  updateMenu(menu: MenuResponse): void {
    this.menu.set(menu);
  }

  /** Zmienia sklep, do którego liczymy listę zakupów (po wyborze w porównywarce). */
  setStore(store: StoreName): void {
    const ob = this.onboarding();
    if (ob) this.onboarding.set({ ...ob, store });
  }

  recipeIds(): number[] {
    return this.menu()?.dishes.map(d => d.recipeId) ?? [];
  }
}

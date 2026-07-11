import { Injectable, computed, signal } from '@angular/core';
import { MenuResponse, OnboardingRequest, StoreName } from './models';
import { readJson, removeKey, writeJson } from './storage';

/** Klucz w localStorage; podbij wersję przy niekompatybilnej zmianie modeli. */
const STORAGE_KEY = 'tanitydzien.plan.v1';

interface PersistedPlan {
  onboarding: OnboardingRequest;
  menu: MenuResponse;
}

/**
 * Trzyma bieżący jadłospis i ustawienia onboardingu między ekranami
 * (jadłospis → lista zakupów → przepis) oraz w localStorage, żeby
 * przeładowanie strony nie gubiło wygenerowanego tygodnia.
 */
@Injectable({ providedIn: 'root' })
export class PlanStateService {
  readonly onboarding = signal<OnboardingRequest | null>(null);
  readonly menu = signal<MenuResponse | null>(null);

  readonly hasMenu = computed(() => this.menu() !== null);

  constructor() {
    const saved = readJson<PersistedPlan>(STORAGE_KEY);
    if (saved?.onboarding && saved?.menu?.dishes) {
      this.onboarding.set(saved.onboarding);
      this.menu.set(saved.menu);
    }
  }

  set(onboarding: OnboardingRequest, menu: MenuResponse): void {
    this.onboarding.set(onboarding);
    this.menu.set(menu);
    this.persist();
  }

  updateMenu(menu: MenuResponse): void {
    this.menu.set(menu);
    this.persist();
  }

  /** Zmienia sklep, do którego liczymy listę zakupów (po wyborze w porównywarce). */
  setStore(store: StoreName): void {
    const ob = this.onboarding();
    if (ob) {
      this.onboarding.set({ ...ob, store });
      this.persist();
    }
  }

  /** Kasuje jadłospis (przycisk „Zacznij od nowa"). */
  clear(): void {
    this.onboarding.set(null);
    this.menu.set(null);
    removeKey(STORAGE_KEY);
  }

  recipeIds(): number[] {
    return this.menu()?.dishes.map(d => d.recipeId) ?? [];
  }

  private persist(): void {
    const onboarding = this.onboarding();
    const menu = this.menu();
    if (onboarding && menu) {
      writeJson(STORAGE_KEY, { onboarding, menu } satisfies PersistedPlan);
    }
  }
}

import { Injectable, computed, signal } from '@angular/core';
import { MenuResponse, OnboardingRequest, StoreName } from './models';

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
    this.restore();
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
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch {
      // localStorage niedostępny (np. tryb prywatny) — stan i tak wyczyszczony w pamięci
    }
  }

  recipeIds(): number[] {
    return this.menu()?.dishes.map(d => d.recipeId) ?? [];
  }

  private persist(): void {
    const onboarding = this.onboarding();
    const menu = this.menu();
    if (!onboarding || !menu) return;
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify({ onboarding, menu } satisfies PersistedPlan));
    } catch {
      // brak miejsca / tryb prywatny — aplikacja działa dalej bez persystencji
    }
  }

  private restore(): void {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return;
      const saved = JSON.parse(raw) as PersistedPlan;
      if (saved?.onboarding && saved?.menu?.dishes) {
        this.onboarding.set(saved.onboarding);
        this.menu.set(saved.menu);
      }
    } catch {
      localStorage.removeItem(STORAGE_KEY);
    }
  }
}

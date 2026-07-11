import { Injectable, signal } from '@angular/core';
import { MenuResponse, OnboardingRequest } from './models';

const STORAGE_KEY = 'tanitydzien.history.v1';
const MAX_ENTRIES = 20;

export interface HistoryEntry {
  id: number;          // timestamp zapisu — wystarcza jako klucz
  savedAt: string;     // ISO — do wyświetlenia
  onboarding: OnboardingRequest;
  menu: MenuResponse;
}

/**
 * Historia wygenerowanych jadłospisów w localStorage (na tym urządzeniu).
 * Najnowsze wpisy na początku; trzymamy maks. 20.
 */
@Injectable({ providedIn: 'root' })
export class HistoryService {
  readonly entries = signal<HistoryEntry[]>(this.load());

  add(onboarding: OnboardingRequest, menu: MenuResponse): void {
    const now = Date.now();
    const entry: HistoryEntry = {
      id: now,
      savedAt: new Date(now).toISOString(),
      onboarding,
      menu
    };
    this.save([entry, ...this.entries()].slice(0, MAX_ENTRIES));
  }

  remove(id: number): void {
    this.save(this.entries().filter(e => e.id !== id));
  }

  clear(): void {
    this.save([]);
  }

  private load(): HistoryEntry[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      const parsed = raw ? JSON.parse(raw) as HistoryEntry[] : [];
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  private save(entries: HistoryEntry[]): void {
    this.entries.set(entries);
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(entries));
    } catch {
      // brak miejsca / tryb prywatny — historia działa tylko w pamięci
    }
  }
}

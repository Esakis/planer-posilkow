import { Injectable, signal } from '@angular/core';
import { MenuResponse, OnboardingRequest } from './models';
import { readJson, writeJson } from './storage';

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
    const parsed = readJson<HistoryEntry[]>(STORAGE_KEY);
    return Array.isArray(parsed) ? parsed : [];
  }

  private save(entries: HistoryEntry[]): void {
    this.entries.set(entries);
    writeJson(STORAGE_KEY, entries);
  }
}

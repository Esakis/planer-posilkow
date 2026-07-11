/**
 * Bezpieczny dostęp do localStorage: każda operacja może rzucić
 * (tryb prywatny, zablokowane cookies, brak quoty), więc wszystkie
 * przechodzą przez try/catch i zwracają neutralne wartości.
 */

export function readJson<T>(key: string): T | null {
  try {
    const raw = localStorage.getItem(key);
    return raw ? JSON.parse(raw) as T : null;
  } catch {
    return null;
  }
}

export function writeJson(key: string, value: unknown): void {
  try {
    localStorage.setItem(key, JSON.stringify(value));
  } catch {
    // brak miejsca / storage zablokowany — aplikacja działa dalej bez persystencji
  }
}

export function removeKey(key: string): void {
  try {
    localStorage.removeItem(key);
  } catch {
    // storage zablokowany — nie ma czego usuwać
  }
}

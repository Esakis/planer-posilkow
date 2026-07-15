import { Ingredient } from './models';

/**
 * Wyszukiwanie składników z podpowiedziami "znajdź cokolwiek":
 * bez polskich znaków ("maka" → "mąka"), każde wpisane słowo dopasowane
 * w dowolnym miejscu nazwy ("piers kurczak" → "pierś z kurczaka"),
 * trafniejsze wyniki wyżej (początek nazwy > początek słowa > środek).
 */

const PL_CHARS: Record<string, string> = {
  ą: 'a', ć: 'c', ę: 'e', ł: 'l', ń: 'n', ó: 'o', ś: 's', ź: 'z', ż: 'z'
};

/** Do porównań wyszukiwania: małe litery i bez polskich znaków. */
export function normalize(text: string): string {
  return text.toLowerCase().replace(/[ąćęłńóśźż]/g, ch => PL_CHARS[ch] ?? ch);
}

export function searchIngredients(
  ingredients: Ingredient[],
  query: string,
  excludeIds?: Set<number>,
  limit = 8
): Ingredient[] {
  const tokens = normalize(query).split(/\s+/).filter(Boolean);
  if (tokens.length === 0) return [];

  const scored: { ing: Ingredient; score: number }[] = [];
  for (const ing of ingredients) {
    if (excludeIds?.has(ing.id)) continue;
    const name = normalize(ing.name);
    if (!tokens.every(t => name.includes(t))) continue;

    const first = tokens[0];
    const score = name.startsWith(first) ? 0
      : name.split(/\s+/).some(w => w.startsWith(first)) ? 1
      : 2;
    scored.push({ ing, score });
  }

  return scored
    .sort((a, b) => a.score - b.score || a.ing.name.localeCompare(b.ing.name, 'pl'))
    .slice(0, limit)
    .map(s => s.ing);
}

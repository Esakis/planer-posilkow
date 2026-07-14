// Typy odpowiadające DTO-om z API (api/Dtos/Dtos.cs).

export type StoreName = 'Biedronka' | 'Lidl' | 'Auchan';

/** Zakresy makro na porcję — null/undefined = bez limitu. */
export interface MacroFilters {
  minProtein?: number | null;
  maxProtein?: number | null;
  minCarbs?: number | null;
  maxCarbs?: number | null;
  minFat?: number | null;
  maxFat?: number | null;
  minKcal?: number | null;
  maxKcal?: number | null;
}

export interface OnboardingRequest {
  people: number;
  weeklyBudget: number;
  store: StoreName;
  exclusions: string[];
  dinners: number;
  macro?: MacroFilters | null;
}

export interface MacroSummary {
  kcal: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface Dish {
  day: number;
  recipeId: number;
  name: string;
  timeMin: number;
  tags: string[];
  kcal: number;
  protein: number;
  carbs: number;
  fat: number;
  cost: number;
  hasPromo: boolean;
}

export interface MenuResponse {
  week: string;
  store: string;
  people: number;
  dinners: number;
  budget: number;
  estimatedCost: number;
  baselineCost: number;
  savings: number;
  overBudget: boolean;
  budgetNote: string | null;
  perDayAvg: MacroSummary;
  dishes: Dish[];
}

export interface IngredientLine {
  ingredient: string;
  product: string;
  grams: number;
  displayQty: string;
  cost: number;
  onPromo: boolean;
  promoNote: string | null;
}

export interface AisleGroup {
  aisle: string;
  items: IngredientLine[];
  subtotal: number;
}

export interface ShoppingList {
  store: string;
  groups: AisleGroup[];
  total: number;
  promoSavings: number;
}

export interface RecipeIngredient {
  name: string;
  displayQty: string;
  aisle: string;
}

export interface RecipeDetail {
  id: number;
  name: string;
  timeMin: number;
  tags: string[];
  servings: number;
  macroPerServing: MacroSummary;
  steps: string[];
  ingredients: RecipeIngredient[];
}

export interface StoreCost {
  store: StoreName;
  kind: 'osiedlowy' | 'hipermarket';
  note: string;
  total: number;
  baseline: number;
  promoSavings: number;
  promoItems: number;
  cheapest: boolean;
  diffToCheapest: number;
}

export interface CompareResponse {
  people: number;
  stores: StoreCost[];
  cheapestStore: StoreName;
  maxSaving: number;
}

/** Składnik z bazy — do wyszukiwarki w formularzu przepisu i własnej liście. */
export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  aisle: string;
  protein100: number;
  carbs100: number;
  fat100: number;
  kcal100: number;
}

export interface CreateRecipeItem {
  ingredientId: number;
  grams: number;
}

/** Własny przepis — ilości składników łączne dla całego przepisu (servings porcji). */
export interface CreateRecipeRequest {
  name: string;
  timeMin: number;
  servings: number;
  tags: string[];
  steps: string[];
  items: CreateRecipeItem[];
}

export interface CustomListItem {
  ingredientId: number;
  grams: number;
}

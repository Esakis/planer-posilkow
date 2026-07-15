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

/** Źródło ceny: verified = gazetka (czarna), user = wpisana (niebieska), predicted = szacunek (szara). */
export type PriceSource = 'predicted' | 'verified' | 'user';

export interface IngredientLine {
  ingredient: string;
  product: string;
  grams: number;
  displayQty: string;
  cost: number;
  onPromo: boolean;
  promoNote: string | null;
  source: PriceSource;
  ingredientId: number;
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
  /** Dodany przez zalogowanego użytkownika — tylko takie można usuwać. */
  mine: boolean;
  favorite: boolean;
  category: string;
}

/** Kategorie dań — spójne z RecipeCategories.All w API. */
export const RECIPE_CATEGORIES: string[] = [
  'Zupy', 'Drób', 'Wieprzowina', 'Wołowina', 'Ryby', 'Wegetariańskie',
  'Makarony', 'Pierogi i mączne', 'Jednogarnkowe i zapiekanki', 'Sałatki'
];

/** Kartka przepisu w katalogu — koszt/makro w kontekście sklepu i liczby osób. */
export interface RecipeCard {
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
  isCustom: boolean;
  mine: boolean;
  favorite: boolean;
  category: string;
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
  /** Suma pozycji porównywalnych (cena zweryfikowana/użytkownika w każdej sieci). */
  verifiedTotal: number;
}

export interface CompareResponse {
  people: number;
  stores: StoreCost[];
  cheapestStore: StoreName;
  maxSaving: number;
  /** Czy różnicę policzono tylko ze zweryfikowanych pozycji (a nie z szacunków). */
  verifiedComparison: boolean;
  verifiedItems: number;
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
  category?: string | null;
}

export interface CustomListItem {
  ingredientId: number;
  grams: number;
}

export interface IngredientPriceInput {
  store: StoreName;
  basePrice: number;
  packSizeG: number;
}

/** Pozycja zapisanej listy zakupów. */
export interface SavedListItem {
  ingredientId: number;
  name: string;
  aisle: string;
  grams: number;
  checked: boolean;
}

/** Zapisana lista zakupów użytkownika (w bazie, powiązana z kontem). */
export interface SavedList {
  id: number;
  name: string;
  updatedAt: string;
  items: SavedListItem[];
}

export interface SavedListItemInput {
  ingredientId: number;
  grams: number;
  checked: boolean;
}

/** Konto użytkownika — plan wyliczany przez API (PlanService). */
export interface Account {
  email: string;
  emailVerified: boolean;
  plan: 'free' | 'trial' | 'premium' | 'expired';
  active: boolean;
  trialDaysLeft: number;
  trialEndsAt: string | null;
}

export interface LoginResponse {
  token: string;
  account: Account;
}

export interface RegisterResponse {
  message: string;
  /** Link aktywacyjny zwracany tylko w dev (mail nie jest realnie wysyłany). */
  devActivationLink: string | null;
}

/** Nowy produkt użytkownika — cena w co najmniej jednym sklepie. */
export interface CreateIngredientRequest {
  name: string;
  aisle: string;
  protein100?: number | null;
  carbs100?: number | null;
  fat100?: number | null;
  kcal100?: number | null;
  prices: IngredientPriceInput[];
}

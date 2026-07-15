import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CompareResponse, CreateIngredientRequest, CreateRecipeRequest, CustomListItem, Ingredient,
  MacroFilters, MenuResponse, OnboardingRequest, RecipeCard, RecipeDetail, SavedList,
  SavedListItemInput, ShoppingList
} from './models';

// względny adres — w dev proxy dev-serwera przekazuje /api do API (web/proxy.conf.json),
// dzięki czemu aplikacja działa też z telefonu w sieci lokalnej (bez CORS i localhost)
const API = '/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  generate(req: OnboardingRequest): Observable<MenuResponse> {
    return this.http.post<MenuResponse>(`${API}/menu/generate`, req);
  }

  swap(body: {
    recipeIds: number[]; swapIndex: number; people: number;
    weeklyBudget: number; store: string; exclusions: string[];
    macro?: MacroFilters | null;
  }): Observable<MenuResponse> {
    return this.http.post<MenuResponse>(`${API}/menu/swap`, body);
  }

  poolCount(body: { exclusions: string[]; macro?: MacroFilters | null }): Observable<{ count: number }> {
    return this.http.post<{ count: number }>(`${API}/menu/pool-count`, body);
  }

  shoppingList(body: {
    recipeIds: number[]; people: number; store: string;
  }): Observable<ShoppingList> {
    return this.http.post<ShoppingList>(`${API}/menu/shopping-list`, body);
  }

  compare(body: { recipeIds: number[]; people: number }): Observable<CompareResponse> {
    return this.http.post<CompareResponse>(`${API}/menu/compare`, body);
  }

  recipe(id: number, people: number): Observable<RecipeDetail> {
    return this.http.get<RecipeDetail>(`${API}/recipes/${id}?people=${people}`);
  }

  recipes(store: string, people: number): Observable<RecipeCard[]> {
    return this.http.get<RecipeCard[]>(`${API}/recipes?store=${store}&people=${people}`);
  }

  addFavorite(recipeId: number): Observable<void> {
    return this.http.post<void>(`${API}/recipes/${recipeId}/favorite`, {});
  }

  removeFavorite(recipeId: number): Observable<void> {
    return this.http.delete<void>(`${API}/recipes/${recipeId}/favorite`);
  }

  ingredients(): Observable<Ingredient[]> {
    return this.http.get<Ingredient[]>(`${API}/ingredients`);
  }

  createIngredient(req: CreateIngredientRequest): Observable<Ingredient> {
    return this.http.post<Ingredient>(`${API}/ingredients`, req);
  }

  /** Cena użytkownika (niebieska) — nadpisuje przewidywaną dla wybranego sklepu. */
  updatePrice(ingredientId: number, body: { store: string; basePrice: number }): Observable<void> {
    return this.http.put<void>(`${API}/ingredients/${ingredientId}/price`, body);
  }

  createRecipe(req: CreateRecipeRequest): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${API}/recipes`, req);
  }

  deleteRecipe(id: number): Observable<void> {
    return this.http.delete<void>(`${API}/recipes/${id}`);
  }

  customList(body: { items: CustomListItem[]; store: string }): Observable<ShoppingList> {
    return this.http.post<ShoppingList>(`${API}/menu/custom-list`, body);
  }

  customCompare(body: { items: CustomListItem[] }): Observable<CompareResponse> {
    return this.http.post<CompareResponse>(`${API}/menu/custom-compare`, body);
  }

  savedLists(): Observable<SavedList[]> {
    return this.http.get<SavedList[]>(`${API}/saved-lists`);
  }

  createSavedList(body: { name: string; items?: SavedListItemInput[] }): Observable<SavedList> {
    return this.http.post<SavedList>(`${API}/saved-lists`, body);
  }

  /** Pełny zapis stanu listy — nazwa i wszystkie pozycje. */
  updateSavedList(id: number, body: { name: string; items: SavedListItemInput[] }): Observable<SavedList> {
    return this.http.put<SavedList>(`${API}/saved-lists/${id}`, body);
  }

  deleteSavedList(id: number): Observable<void> {
    return this.http.delete<void>(`${API}/saved-lists/${id}`);
  }
}

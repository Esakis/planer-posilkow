import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CompareResponse, CreateIngredientRequest, CreateRecipeRequest, CustomListItem, Ingredient,
  MacroFilters, MenuResponse, OnboardingRequest, RecipeDetail, ShoppingList
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

  ingredients(): Observable<Ingredient[]> {
    return this.http.get<Ingredient[]>(`${API}/ingredients`);
  }

  createIngredient(req: CreateIngredientRequest): Observable<Ingredient> {
    return this.http.post<Ingredient>(`${API}/ingredients`, req);
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
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CompareResponse, MenuResponse, OnboardingRequest, RecipeDetail, ShoppingList
} from './models';

const API = 'http://localhost:5080/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  generate(req: OnboardingRequest): Observable<MenuResponse> {
    return this.http.post<MenuResponse>(`${API}/menu/generate`, req);
  }

  swap(body: {
    recipeIds: number[]; swapIndex: number; people: number;
    weeklyBudget: number; store: string; exclusions: string[];
    minProteinPerServing?: number | null; maxKcalPerServing?: number | null;
  }): Observable<MenuResponse> {
    return this.http.post<MenuResponse>(`${API}/menu/swap`, body);
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
}

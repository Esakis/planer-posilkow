import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/onboarding.component').then(m => m.OnboardingComponent)
  },
  {
    path: 'menu',
    loadComponent: () =>
      import('./pages/menu.component').then(m => m.MenuComponent)
  },
  {
    path: 'compare',
    loadComponent: () =>
      import('./pages/compare.component').then(m => m.CompareComponent)
  },
  {
    path: 'shopping',
    loadComponent: () =>
      import('./pages/shopping.component').then(m => m.ShoppingComponent)
  },
  {
    path: 'recipe/:id',
    loadComponent: () =>
      import('./pages/recipe.component').then(m => m.RecipeComponent)
  },
  {
    path: 'history',
    loadComponent: () =>
      import('./pages/history.component').then(m => m.HistoryComponent)
  },
  {
    path: 'add-recipe',
    loadComponent: () =>
      import('./pages/add-recipe.component').then(m => m.AddRecipeComponent)
  },
  {
    path: 'my-list',
    loadComponent: () =>
      import('./pages/my-list.component').then(m => m.MyListComponent)
  },
  { path: '**', redirectTo: '' }
];

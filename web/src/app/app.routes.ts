import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/onboarding.component').then(m => m.OnboardingComponent)
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'activate',
    loadComponent: () =>
      import('./pages/activate.component').then(m => m.ActivateComponent)
  },
  {
    path: 'account',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/account.component').then(m => m.AccountComponent)
  },
  {
    path: 'menu',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/menu.component').then(m => m.MenuComponent)
  },
  {
    path: 'compare',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/compare.component').then(m => m.CompareComponent)
  },
  {
    path: 'shopping',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/shopping.component').then(m => m.ShoppingComponent)
  },
  {
    path: 'recipes',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/recipes.component').then(m => m.RecipesComponent)
  },
  {
    path: 'recipe/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/recipe.component').then(m => m.RecipeComponent)
  },
  {
    path: 'history',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/history.component').then(m => m.HistoryComponent)
  },
  {
    path: 'add-recipe',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/add-recipe.component').then(m => m.AddRecipeComponent)
  },
  {
    path: 'my-list',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/my-list.component').then(m => m.MyListComponent)
  },
  { path: '**', redirectTo: '' }
];

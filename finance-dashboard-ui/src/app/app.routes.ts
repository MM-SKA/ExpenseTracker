import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent:()=>import('./features/auth/login/login.component').then(m=>m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent:()=>import('./features/auth/register/register').then(m=>m.Register)
  },
  {
    path: 'categories',
    loadComponent:()=>import('./features/categories/category-list/category-list').then(m=>m.CategoryList),
    canActivate: [authGuard]
  },
  {
    path: 'categories/create',
    loadComponent:()=>import('./features/categories/create-category/create-category').then(m=>m.CreateCategory),
    canActivate: [authGuard]
  },
  {
    path: 'categories/edit/:id',
    loadComponent:()=>import('./features/categories/update-category/update-category').then(m=>m.UpdateCategory),
    canActivate: [authGuard]
  },
  {
    path: 'expenses',
    loadComponent: () => import('./features/expenses/expense-list/expense-list').then(m => m.ExpenseList),
    canActivate: [authGuard]
  },
  {
    path: 'expenses/create',
    loadComponent:()=>import('./features/expenses/create-expense/create-expense').then(m=>m.CreateExpense),
    canActivate: [authGuard]
  },
  {
    path: 'analytics',
    loadComponent:()=>import('./features/analytics/analytics-dashboard/analytics-dashboard').then(m=>m.AnalyticsDashboard),
    canActivate: [authGuard]
  },
  {
    path: 'ai',
    loadComponent:()=>import('./features/ai/ai-chat/ai-chat').then(m=>m.AiChat),
    canActivate: [authGuard]
  },
  {
    path: 'expenses/edit/:id',
    loadComponent:()=>import('./features/expenses/edit-expense/edit-expense').then(m=>m.EditExpense),
    canActivate: [authGuard]
  }
];

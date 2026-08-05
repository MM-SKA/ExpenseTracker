import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { CreateCategory } from './features/categories/create-category/create-category';
import { LoginComponent } from './features/auth/login/login.component';
import { Register } from './features/auth/register/register';
import { CategoryList } from './features/categories/category-list/category-list';
import { UpdateCategory } from './features/categories/update-category/update-category';
import { AnalyticsDashboard } from './features/analytics/analytics-dashboard/analytics-dashboard';
import { CreateExpense } from './features/expenses/create-expense/create-expense';
import { ExpenseList } from './features/expenses/expense-list/expense-list';

export const routes: Routes = [
  {
    path: '',
    component: LoginComponent
  },
  {
    path: 'register',
    component: Register
  },
  {
    path: 'categories',
    component: CategoryList,
    canActivate: [authGuard]
  },
  {
    path: 'categories/create',
    component: CreateCategory,
    canActivate: [authGuard]
  },
  {
    path: 'categories/edit/:id',
    component: UpdateCategory,
    canActivate: [authGuard]
  },
  {
    path: 'expenses',
    component: ExpenseList,
    canActivate: [authGuard]
  },
  {
    path: 'expenses/create',
    component: CreateExpense,
    canActivate: [authGuard]
  },
  {
    path: 'analytics',
    component: AnalyticsDashboard,
    canActivate: [authGuard]
  }
];

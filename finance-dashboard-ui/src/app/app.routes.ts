import { Routes } from '@angular/router';
import { CreateCategory } from './features/categories/create-category/create-category';

import { LoginComponent } from './features/auth/login/login.component';
import { CategoryList } from './features/categories/category-list/category-list';
import { UpdateCategory } from './features/categories/update-category/update-category';
// import { Dashboard } from './features/dashboard/dashboard';
import { CreateExpense } from './features/expenses/create-expense/create-expense';
import { ExpenseList } from './features/expenses/expense-list/expense-list';

export const routes: Routes = [

  {
    path: '',
    component: LoginComponent
  },
  {
    path: 'categories',

    component: CategoryList

  },
  {
    path: 'categories/create',
    component: CreateCategory
  },
  {
    path: 'categories/edit/:id',
    component: UpdateCategory
  }
  ,
  {
    path: 'expenses',
    component: ExpenseList
  },
  {
    path: 'expenses/create',
    component: CreateExpense
  }
  // {
  // path: 'dashboard',
  // component: DashboardComponent
  // }

];

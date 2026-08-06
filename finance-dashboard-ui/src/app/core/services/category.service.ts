import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, tap, catchError } from 'rxjs';
import { Category } from '../../shared/models/category/category';
import { CreateCategoryRequest } from '../../shared/models/category/create-category-request';
import { UpdateCategoryRequest } from '../../shared/models/category/update-category-request';

import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  readonly http = inject(HttpClient);
  private categoriesCache: Category[] | null = null;
  readonly storageKey = 'categories_cache';

  getCategories(forceRefresh = false): Observable<Category[]> {
    if (!forceRefresh && this.categoriesCache?.length) {
      return of(this.categoriesCache);
    }

    const cached = this.loadFromStorage();
    if (!forceRefresh && cached.length) {
      this.categoriesCache = cached;
      return of(cached);
    }

    return this.http.get<Category[]>(`${environment.apiUrl}/categories/get`).pipe(
      tap((categories) => {
        this.categoriesCache = Array.isArray(categories) ? categories : [];
        this.saveToStorage(this.categoriesCache);
      }),
      catchError(() => {
        return of(this.categoriesCache ?? cached ?? []);
      })
    );
  }

  refreshCategories(): Observable<Category[]> {
    return this.getCategories(true);
  }

  clearCache(): void {
    this.categoriesCache = null;
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem(this.storageKey);
    }
  }

  private loadFromStorage(): Category[] {
    if (typeof localStorage === 'undefined') {
      return [];
    }
    try {
      const raw = localStorage.getItem(this.storageKey);
      if (!raw) {
        return [];
      }
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(categories: Category[]): void {
    if (typeof localStorage === 'undefined') {
      return;
    }
    try {
      localStorage.setItem(this.storageKey, JSON.stringify(categories));
    } catch {
      // ignore failures
    }
  }

  createCategory(request: CreateCategoryRequest) {
    return this.http.post(
      `${environment.apiUrl}/categories/create`,
      request
    );
  }

  updateCategory(id: string, request: UpdateCategoryRequest) {
    return this.http.put(
      `${environment.apiUrl}/categories/update/${id}`,
      request
    );
  }

  getCategoryById(id: string) {
    return this.http.get<Category>(
      `${environment.apiUrl}/categories/${id}`
    );
  }

  refreshCategoriesCache(): void {
    this.getCategories(true)
      .subscribe();
  }
}

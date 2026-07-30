import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../shared/models/category/category';
import { CreateCategoryRequest } from '../../shared/models/category/create-category-request';
import { UpdateCategoryRequest } from '../../shared/models/category/update-category-request';

import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);

  getCategories() {
    return this.http.get<Category[]>(
      `${environment.apiUrl}/categories/get`
    );
  }

  createCategory(
    request: CreateCategoryRequest
  ) {

    return this.http.post(
      `${environment.apiUrl}/categories/create`,
      request
    );
  }

  updateCategory(
    id: string,
    request: UpdateCategoryRequest
  ) {

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
}

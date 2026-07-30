import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../shared/models/category/category';

import { environment } from '../../../environment/environment';

@Injectable({
  providedIn:'root'
})
export class CategoryService{
  private http = inject(HttpClient);

  getCategories(){
    return this.http.get<Category[]>(
      `${environment.apiUrl}/categories/get`
    );
  }
}

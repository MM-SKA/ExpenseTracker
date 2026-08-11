import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environment/environment';
import { CreateExpenseRequest } from '../../shared/models/expense/create-expense-request';
import { UpdateExpenseRequest } from '../../shared/models/expense/update-expense-request';
import { PaginationResponse } from '../../shared/models/expense/pagination-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  readonly http = inject(HttpClient);
  createExpense(request: CreateExpenseRequest) {
    return this.http.post(
      `${environment.apiUrl}/expenses/create`,
      request
    );
  }

  getExpenses() {
    return this.http.get(`${environment.apiUrl}/expenses/get`);
  }

  filterExpenses(filter: any) {
    return this.http.post(`${environment.apiUrl}/expenses/filter`, filter);
  }

  getExpenseById(id: string) {
    return this.http.get(`${environment.apiUrl}/expenses/get/${id}`);
  }

  updateExpense(id: string, request: UpdateExpenseRequest) {
    return this.http.put(
      `${environment.apiUrl}/expenses/update/${id}`,
      request
    );
  }

  deleteExpense(id: string) {
    return this.http.delete(
      `${environment.apiUrl}/expenses/delete/${id}`
    );
  }

  getPagedExpenses(
    pageNumber: number,
    pageSize: number
  ) {
    return this.http.get<PaginationResponse<any>>(
      `${environment.apiUrl}/expenses/paged`,
      {
        params: {
          pageNumber,
          pageSize
        }
      }
    );
  }

  filterPagedExpenses(request: ExpenseFilterRequest) {
  return this.http.post<any>(
    `${environment.apiUrl}/expenses/filter-paged`,
    request
  );
}

}

export interface ExpenseFilterRequest {
  pageNumber: number;
  pageSize: number;
  categoryId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  minAmount?: number | null;
  maxAmount?: number | null;
  notes?: string | null;
  location?: string | null;
  sortOrder: 'recent' | 'oldest';
  includeAnalytics: boolean;
}

export interface FilteredPagedExpenseResponse {
  items: any[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  analytics?: any;
}

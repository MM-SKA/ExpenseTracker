import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environment/environment';
import { CreateExpenseRequest } from '../../shared/models/expense/create-expense-request';
import { UpdateExpenseRequest } from '../../shared/models/expense/update-expense-request';
import { PaginationResponse } from '../../shared/models/expense/pagination-response';
import { ExpenseDto } from '../../shared/models/expense/expense-generic';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../shared/models/api-response';

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

  getExpenses(): Observable<ExpenseDto[]> {
    return this.http.get<ExpenseDto[]>(
      `${environment.apiUrl}/expenses/get`
    );
  }


  filterExpenses(filter: any) {
    return this.http.post(`${environment.apiUrl}/expenses/filter`, filter);
  }

  getExpenseById(
    id: string
  ): Observable<ApiResponse<ExpenseDto>> {

    return this.http.get<ApiResponse<ExpenseDto>>(
      `${environment.apiUrl}/expenses/get/${id}`
    );

  }

  updateExpense(
    id: string,
    request: UpdateExpenseRequest
  ): Observable<ApiResponse<ExpenseDto>> {

    return this.http.put<ApiResponse<ExpenseDto>>(
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

  exportExpenses(
    request: ExpenseFilterRequest
  ) {
    return this.http.post(
      `${environment.apiUrl}/expenses/export`,
      request,
      {
        responseType: 'blob',
        withCredentials: true
      }
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

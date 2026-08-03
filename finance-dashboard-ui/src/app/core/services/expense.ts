import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environment/environment';

import { CreateExpenseRequest } from '../../shared/models/expense/create-expense-request';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {

  readonly http =
    inject(HttpClient);

  createExpense(
    request: CreateExpenseRequest
  ) {

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

}

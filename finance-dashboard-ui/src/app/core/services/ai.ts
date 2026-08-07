import {
  Injectable,
  inject
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  environment
} from '../../../environment/environment';

export interface AskExpenseAnswer {

  answer: string;

  filtersUsed: any;

  analytics: any;

  expenses: any;

}

@Injectable({
  providedIn: 'root'
})
export class AiService {

  private readonly http =
    inject(HttpClient);

  askExpenses(
    request: {
      question: string;
    }
  ): Observable<AskExpenseAnswer> {

    return this.http.post<AskExpenseAnswer>(

      `${environment.apiUrl}/ai/ask-expenses`,

      request

    );

  }

}

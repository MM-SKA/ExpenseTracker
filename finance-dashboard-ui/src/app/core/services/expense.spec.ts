import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { provideHttpClient } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { environment } from "../../../environment/environment";
import { ExpenseService } from './expense';
import { CreateExpenseRequest } from "../../shared/models/expense/create-expense-request";

describe('ExpenseService', () => {
  let service: ExpenseService;
  let httpMock: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })

    service = TestBed.inject(ExpenseService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => {
    httpMock.verify();
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should create an expense", () => {
    const request: CreateExpenseRequest = {
      categoryId: 'category-1',
      amount: 500,
      notes: 'Lunch',
      date: '2026-08-18',
      location: 'Vadodara'
    };

    const mockResponse = {
      success: true,
      message: 'Expense created successfully'
    };

    service.createExpense(request).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(
      `${environment.apiUrl}/expenses/create`
    );

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);

    req.flush(mockResponse);
  });

  it('should get all expenses', () => {

    const responseBody = [
      {
        id: 'expense-1',
        description: 'Lunch',
        amount: 500,
        date: '2026-08-18',
        categoryId: 'category-1',
        categoryName: 'Food',
        location: 'Vadodara'
      }
    ];

    service
      .getExpenses()
      .subscribe(response => {

        expect(response)
          .toEqual(responseBody);

        expect(response)
          .toHaveLength(1);

        expect(response[0].description)
          .toBe('Lunch');

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/get`
      );

    expect(request.request.method)
      .toBe('GET');

    request.flush(responseBody);

  });

  it('should get expense by id', () => {

    const expenseId = 'expense-1';

    const responseBody = {
      success: true,
      message: 'Expense found',
      errorCode: null,
      data: {
        id: expenseId,
        description: 'Lunch',
        amount: 500,
        date: '2026-08-18',
        categoryId: 'category-1',
        categoryName: 'Food',
        location: 'Vadodara'
      }
    };

    service
      .getExpenseById(expenseId)
      .subscribe(response => {

        expect(response)
          .toEqual(responseBody);

        expect(response.data.id)
          .toBe(expenseId);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/get/${expenseId}`
      );

    expect(request.request.method)
      .toBe('GET');

    request.flush(responseBody);

  });

  it('should update an expense', () => {

    const expenseId = 'expense-1';

    const requestBody = {
      categoryId: 'category-1',
      amount: 800,
      notes: 'Dinner',
      date: '2026-08-18',
      location: 'Vadodara'
    };

    const responseBody = {
      success: true,
      message: 'Expense updated successfully',
      errorCode: null,
      data: {
        id: expenseId,
        description: 'Dinner',
        amount: 800,
        date: '2026-08-18',
        categoryId: 'category-1',
        categoryName: 'Food',
        location: 'Vadodara'
      }
    };

    service
      .updateExpense(
        expenseId,
        requestBody
      )
      .subscribe(response => {

        expect(response.success)
          .toBe(true);

        expect(response.data?.id)
          .toBe(expenseId);

        expect(response.data?.description)
          .toBe('Dinner');

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/update/${expenseId}`
      );

    expect(request.request.method)
      .toBe('PUT');

    expect(request.request.body)
      .toEqual(requestBody);

    request.flush(responseBody);

  });

  it('should propagate update expense error', () => {

    const expenseId = 'expense-1';

    const requestBody = {
      categoryId: 'invalid-category',
      amount: 800,
      notes: 'Dinner',
      date: '2026-08-18',
      location: 'Vadodara'
    };

    service
      .updateExpense(
        expenseId,
        requestBody
      )
      .subscribe({
        next: () => {
          throw new Error(
            'Expected update request to fail'
          );
        },

        error: error => {

          expect(error.status)
            .toBe(400);

        }
      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/update/${expenseId}`
      );

    request.flush(
      {
        success: false,
        message: 'Invalid category',
        errorCode: 'INVALID_CATEGORY'
      },
      {
        status: 400,
        statusText: 'Bad Request'
      }
    );

  });

  it('should delete an expense', () => {

    const expenseId = 'expense-1';

    const responseBody = {
      success: true,
      message: 'Expense deleted successfully',
      errorCode: null
    };

    service
      .deleteExpense(expenseId)
      .subscribe(response => {

        expect(response)
          .toEqual(responseBody);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/delete/${expenseId}`
      );

    expect(request.request.method)
      .toBe('DELETE');

    request.flush(responseBody);

  });
  it('should propagate delete expense error', () => {

    const expenseId = 'expense-not-found';

    service
      .deleteExpense(expenseId)
      .subscribe({
        next: () => {
          throw new Error(
            'Expected delete request to fail'
          );
        },

        error: error => {

          expect(error.status)
            .toBe(404);

        }
      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/delete/${expenseId}`
      );

    request.flush(
      {
        success: false,
        message: 'Expense not found',
        errorCode: 'EXPENSE_NOT_FOUND'
      },
      {
        status: 404,
        statusText: 'Not Found'
      }
    );

  });

  it('should propagate get expense by id error', () => {

    const expenseId = 'invalid-id';

    service
      .getExpenseById(expenseId)
      .subscribe({
        next: () => {
          throw new Error(
            'Expected request to fail'
          );
        },

        error: error => {

          expect(error.status)
            .toBe(404);

        }
      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/get/${expenseId}`
      );

    request.flush(
      {
        success: false,
        message: 'Expense not found',
        errorCode: 'EXPENSE_NOT_FOUND'
      },
      {
        status: 404,
        statusText: 'Not Found'
      }
    );

  });

  it('should filter expenses', () => {

    const filter = {
      categoryId: 'category-1',
      minAmount: 100
    };

    const response = [
      {
        id: 'expense-1',
        description: 'Lunch'
      }
    ];

    service
      .filterExpenses(filter)
      .subscribe(result => {

        expect(result)
          .toEqual(response);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/filter`
      );

    expect(request.request.method)
      .toBe('POST');

    expect(request.request.body)
      .toEqual(filter);

    request.flush(response);

  });

  it('should get paged expenses', () => {

    const response = {
      items: [
        {
          id: 'expense-1'
        }
      ],
      pageNumber: 1,
      pageSize: 10,
      totalRecords: 50,
      totalPages: 5
    };

    service
      .getPagedExpenses(
        1,
        10
      )
      .subscribe(result => {

        expect(result.items)
          .toHaveLength(1);

        expect(result.totalPages)
          .toBe(5);

      });

    const request =
      httpMock.expectOne(
        req =>
          req.url ===
          `${environment.apiUrl}/expenses/paged`
      );

    expect(request.request.method)
      .toBe('GET');

    expect(
      request.request.params.get('pageNumber')
    ).toBe('1');

    expect(
      request.request.params.get('pageSize')
    ).toBe('10');

    request.flush(response);

  });

  it('should filter paged expenses', () => {

    const filterRequest = {
      pageNumber: 1,
      pageSize: 10,
      sortOrder: 'recent' as const,
      includeAnalytics: true
    };

    const response = {
      items: [],
      pageNumber: 1,
      pageSize: 10,
      totalRecords: 0,
      totalPages: 1
    };

    service
      .filterPagedExpenses(
        filterRequest
      )
      .subscribe(result => {

        expect(result)
          .toEqual(response);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/filter-paged`
      );

    expect(request.request.method)
      .toBe('POST');

    expect(request.request.body)
      .toEqual(filterRequest);

    request.flush(response);

  });

  it('should export expenses as excel blob', () => {

    const filterRequest = {
      pageNumber: 1,
      pageSize: 100,
      sortOrder: 'recent' as const,
      includeAnalytics: false
    };

    const blob =
      new Blob(
        ['excel-data'],
        {
          type:
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        }
      );

    service
      .exportExpenses(
        filterRequest
      )
      .subscribe(result => {

        expect(result)
          .toBeInstanceOf(Blob);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/expenses/export`
      );

    expect(request.request.method)
      .toBe('POST');

    expect(request.request.body)
      .toEqual(filterRequest);

    expect(request.request.withCredentials)
      .toBe(true);

    expect(request.request.responseType)
      .toBe('blob');

    request.flush(blob);

  });

});

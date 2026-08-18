import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

import {
  provideHttpClient
} from '@angular/common/http';

import {
  TestBed
} from '@angular/core/testing';

import {
  afterEach,
  beforeEach,
  describe,
  expect,
  it,
  vi
} from 'vitest';

import {
  CategoryService
} from './category.service';

import {
  StorageService
} from './storage.service';

import {
  Category
} from '../../shared/models/category/category';

import {
  environment
} from '../../../environment/environment';

describe('CategoryService', () => {

  let service: CategoryService;

  let httpMock: HttpTestingController;

  let storageServiceMock: {
    getEncrypted: ReturnType<typeof vi.fn>;
    setEncrypted: ReturnType<typeof vi.fn>;
  };

  beforeEach(() => {

    storageServiceMock = {
      getEncrypted: vi.fn(),
      setEncrypted: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: StorageService,
          useValue: storageServiceMock
        }
      ]
    });

    service =
      TestBed.inject(CategoryService);

    httpMock =
      TestBed.inject(HttpTestingController);

  });

  afterEach(() => {

    httpMock.verify();

  });
  it('should be created', () => {

    expect(service)
      .toBeTruthy();

  });
  it('should get categories from api', () => {

    const categories: Category[] = [
      {
        id: '1',
        name: 'Food',
        isSystemCategory: true
      }
    ];

    storageServiceMock.getEncrypted
      .mockReturnValue([]);

    service
      .getCategories()
      .subscribe(response => {

        expect(response)
          .toEqual(categories);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/get`
      );

    expect(request.request.method)
      .toBe('GET');

    request.flush(categories);

  });
  it('should return cached categories', () => {

    const categories: Category[] = [
      {
        id: '1',
        name: 'Food',
        isSystemCategory: true
      }
    ];

    storageServiceMock.getEncrypted
      .mockReturnValue([]);

    service
      .getCategories()
      .subscribe();

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/get`
      );

    request.flush(categories);

    service
      .getCategories()
      .subscribe(response => {

        expect(response)
          .toEqual(categories);

      });

  });
  it('should return categories from storage', () => {

    const categories: Category[] = [
      {
        id: '1',
        name: 'Food',
        isSystemCategory: true
      }
    ];

    storageServiceMock.getEncrypted
      .mockReturnValue(categories);

    service
      .getCategories()
      .subscribe(response => {

        expect(response)
          .toEqual(categories);

      });

    httpMock.expectNone(
      `${environment.apiUrl}/categories/get`
    );

  });

  it('should create category', () => {

    service
      .createCategory({
        name: 'Food'
      })
      .subscribe();

    const createRequest =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/create`
      );

    expect(createRequest.request.method)
      .toBe('POST');

    createRequest.flush({});

    const refreshRequest =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/get`
      );

    refreshRequest.flush([]);

  });
  it('should update category', () => {

    service
      .updateCategory(
        '1',
        {
          name: 'Updated Food'
        }
      )
      .subscribe();

    const updateRequest =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/update/1`
      );

    expect(updateRequest.request.method)
      .toBe('PUT');

    updateRequest.flush({});

    const refreshRequest =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/get`
      );

    refreshRequest.flush([]);

  });
  it('should update category', () => {

    service
      .updateCategory(
        '1',
        {
          name: 'Updated Food'
        }
      )
      .subscribe();

    const updateRequest =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/update/1`
      );

    expect(updateRequest.request.method)
      .toBe('PUT');

    updateRequest.flush({});

    const refreshRequest =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/get`
      );

    refreshRequest.flush([]);

  });
  it('should delete category', () => {

    service
      .deleteCategory('1')
      .subscribe();

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/delete/1`
      );

    expect(request.request.method)
      .toBe('DELETE');

    request.flush({});

  });
  it('should refresh categories', () => {

    service
      .refreshCategories()
      .subscribe();

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/categories/get`
      );

    request.flush([]);

  });
  it('should clear cache', () => {

    localStorage.setItem(
      service.storageKey,
      'test-value'
    );

    service.clearCache();

    expect(
      localStorage.getItem(
        service.storageKey
      )
    ).toBeNull();

  });
})

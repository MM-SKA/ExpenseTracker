import {
  HttpErrorResponse,
  HttpHandlerFn,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';

import {
  TestBed
} from '@angular/core/testing';

import {
  provideRouter,
  Router
} from '@angular/router';

import {
  firstValueFrom,
  of,
  Subject,
  throwError
} from 'rxjs';

import {
  afterEach,
  beforeEach,
  describe,
  expect,
  it,
  vi
} from 'vitest';

import {
  AuthService
} from '../services/auth.service';

import {
  authInterceptor
} from './auth-interceptor';

describe('authInterceptor', () => {

  let authServiceMock: {
    refreshToken: ReturnType<typeof vi.fn>;
  };

  let router: Router;
  let navigateSpy: ReturnType<typeof vi.spyOn>;

  const executeInterceptor = (
    req: HttpRequest<unknown>,
    next: HttpHandlerFn
  ) => {
    return TestBed.runInInjectionContext(
      () => authInterceptor(req, next)
    );
  };

  beforeEach(() => {

    authServiceMock = {
      refreshToken: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: authServiceMock
        }
      ]
    });

    router =
      TestBed.inject(Router);

    navigateSpy =
      vi.spyOn(
        router,
        'navigate'
      ).mockResolvedValue(true);

  });

  afterEach(() => {

    vi.restoreAllMocks();

  });

  it('should be created', () => {

    expect(authInterceptor)
      .toBeTruthy();

  });

  it('should attach withCredentials to every outgoing request', async () => {

    const request =
      new HttpRequest(
        'GET',
        '/api/expenses'
      );

    const next: HttpHandlerFn =
      vi.fn(
        clonedRequest =>
          of(
            new HttpResponse({
              status: 200,
              body: {
                success: true
              }
            })
          )
      );

    await firstValueFrom(
      executeInterceptor(
        request,
        next
      )
    );

    expect(next)
      .toHaveBeenCalledTimes(1);

    const clonedRequest =
      vi.mocked(next).mock.calls[0][0];

    expect(clonedRequest.withCredentials)
      .toBe(true);

  });

  it('should rethrow non-401 errors without refreshing token', async () => {

    const request =
      new HttpRequest(
        'GET',
        '/api/expenses'
      );

    const serverError =
      new HttpErrorResponse({
        status: 500,
        statusText: 'Server Error',
        url: '/api/expenses'
      });

    const next: HttpHandlerFn =
      vi.fn(
        () => throwError(
          () => serverError
        )
      );

    await expect(
      firstValueFrom(
        executeInterceptor(
          request,
          next
        )
      )
    ).rejects.toBe(serverError);

    expect(authServiceMock.refreshToken)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

  });

  it.each([
    '/api/auth/login',
    '/api/auth/register',
    '/api/auth/refresh'
  ])(
    'should not refresh token for auth endpoint %s when 401 occurs',
    async (url) => {

      const request =
        new HttpRequest(
          'POST',
          url,
          {}
        );

      const unauthorizedError =
        new HttpErrorResponse({
          status: 401,
          statusText: 'Unauthorized',
          url
        });

      const next: HttpHandlerFn =
        vi.fn(
          () => throwError(
            () => unauthorizedError
          )
        );

      await expect(
        firstValueFrom(
          executeInterceptor(
            request,
            next
          )
        )
      ).rejects.toBe(unauthorizedError);

      expect(authServiceMock.refreshToken)
        .not
        .toHaveBeenCalled();

      expect(navigateSpy)
        .toHaveBeenCalledWith([
          '/'
        ]);

    }
  );


  it('should navigate to login when refresh token request fails', async () => {

    const request =
      new HttpRequest(
        'GET',
        '/api/expenses'
      );

    const unauthorizedError =
      new HttpErrorResponse({
        status: 401,
        statusText: 'Unauthorized',
        url: '/api/expenses'
      });

    const refreshError =
      new HttpErrorResponse({
        status: 401,
        statusText: 'Refresh Failed',
        url: '/api/auth/refresh'
      });

    const next: HttpHandlerFn =
      vi.fn(
        () => throwError(
          () => unauthorizedError
        )
      );

    authServiceMock.refreshToken
      .mockReturnValue(
        throwError(
          () => refreshError
        )
      );

    await expect(
      firstValueFrom(
        executeInterceptor(
          request,
          next
        )
      )
    ).rejects.toBe(refreshError);

    expect(authServiceMock.refreshToken)
      .toHaveBeenCalledTimes(1);

    expect(navigateSpy)
      .toHaveBeenCalledWith([
        '/'
      ]);

  });

  it('should wait for existing refresh request and then retry when another request is already refreshing', async () => {

    const refreshSubject =
      new Subject<unknown>();

    const firstRequest =
      new HttpRequest(
        'GET',
        '/api/expenses'
      );

    const secondRequest =
      new HttpRequest(
        'GET',
        '/api/categories'
      );

    const callCountByUrl =
      new Map<string, number>();

    const next: HttpHandlerFn =
      vi.fn(
        request => {

          const previousCount =
            callCountByUrl.get(request.url) ?? 0;

          const currentCount =
            previousCount + 1;

          callCountByUrl.set(
            request.url,
            currentCount
          );

          if (currentCount === 1) {
            return throwError(
              () => new HttpErrorResponse({
                status: 401,
                statusText: 'Unauthorized',
                url: request.url
              })
            );
          }

          return of(
            new HttpResponse({
              status: 200,
              body: {
                url: request.url
              }
            })
          );

        }
      );

    authServiceMock.refreshToken
      .mockReturnValue(
        refreshSubject.asObservable()
      );

    const firstPromise =
      firstValueFrom(
        executeInterceptor(
          firstRequest,
          next
        )
      );

    const secondPromise =
      firstValueFrom(
        executeInterceptor(
          secondRequest,
          next
        )
      );

    expect(authServiceMock.refreshToken)
      .toHaveBeenCalledTimes(1);

    refreshSubject.next({
      success: true
    });

    refreshSubject.complete();

    const results =
      await Promise.all([
        firstPromise,
        secondPromise
      ]);

    expect(results)
      .toHaveLength(2);

    expect(next)
      .toHaveBeenCalledTimes(4);

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

  });

});

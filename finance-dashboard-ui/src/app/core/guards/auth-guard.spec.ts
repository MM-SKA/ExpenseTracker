import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, UrlTree } from '@angular/router';
import { firstValueFrom, Observable, of, throwError } from 'rxjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { AuthService } from '../services/auth.service';
import { authGuard } from './auth-guard';

describe('authGuard', () => {

  let authServiceMock: {
    getCurrentUser: ReturnType<typeof vi.fn>;
  };

  let router: Router;

  const executeGuard = () =>
    TestBed.runInInjectionContext(
      () => authGuard(
        {} as never,
        {} as never
      )
    ) as Observable<boolean | UrlTree>;

  beforeEach(() => {

    authServiceMock = {
      getCurrentUser: vi.fn()
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

  });

  it('should be created', () => {

    expect(authGuard)
      .toBeTruthy();

  });

  it('should allow navigation when current user exists', async () => {

    authServiceMock.getCurrentUser
      .mockReturnValue(
        of({
          id: '1',
          email: 'test@test.com'
        })
      );

    const result =
      await firstValueFrom(
        executeGuard()
      );

    expect(result)
      .toBe(true);

    expect(authServiceMock.getCurrentUser)
      .toHaveBeenCalledTimes(1);

  });

  it('should redirect to login when current user request fails', async () => {

    authServiceMock.getCurrentUser
      .mockReturnValue(
        throwError(
          () => new Error('Unauthorized')
        )
      );

    const result =
      await firstValueFrom(
        executeGuard()
      );

    expect(result)
      .toBeInstanceOf(UrlTree);

    expect(
      router.serializeUrl(
        result as UrlTree
      )
    ).toBe('/');

    expect(authServiceMock.getCurrentUser)
      .toHaveBeenCalledTimes(1);

  });

});

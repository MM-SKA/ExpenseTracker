import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  BehaviorSubject,
  Observable,
  catchError,
  filter,
  switchMap,
  take,
  throwError
} from 'rxjs';
import { AuthService } from '../services/auth.service';

// Shared refresh state across all interceptor calls
let isRefreshing = false;
const refreshDone$ = new BehaviorSubject<boolean>(false);

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  // Always attach credentials (cookies) to every outgoing request
  const cloned = req.clone({ withCredentials: true });

  return next(cloned).pipe(

    catchError((error: unknown): Observable<HttpEvent<unknown>> => {

      // Only handle 401 Unauthorized errors
      if (!(error instanceof HttpErrorResponse) || error.status !== 401) {
        return throwError(() => error);
      }

      // Don't retry refresh/login/register endpoints — avoid infinite loops
      const url = req.url;
      if (
        url.includes('/auth/refresh') ||
        url.includes('/auth/login') ||
        url.includes('/auth/register')
      ) {
        isRefreshing = false;
        refreshDone$.next(false);
        void router.navigate(['/']);
        return throwError(() => error);
      }

      if (isRefreshing) {
        // Another request already triggered a refresh — wait for it to complete
        return refreshDone$.pipe(
          filter(done => done),
          take(1),
          switchMap(() => next(cloned))
        );
      }

      isRefreshing = true;
      refreshDone$.next(false);

      return authService.refreshToken().pipe(

        switchMap((): Observable<HttpEvent<unknown>> => {
          isRefreshing = false;
          refreshDone$.next(true);
          // Retry the original request with the new cookies in place
          return next(cloned);
        }),

        catchError((refreshError: unknown) => {
          isRefreshing = false;
          refreshDone$.next(false);
          void router.navigate(['/']);
          return throwError(() => refreshError);
        })

      );
    })

  );
};

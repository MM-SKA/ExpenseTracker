import { EnvironmentProviders, Provider } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

import { provideToastr } from 'ngx-toastr';

import {
  provideTranslateService,
  TranslateLoader
} from '@ngx-translate/core';

import { Observable, of } from 'rxjs';

class FakeTranslateLoader implements TranslateLoader {
  getTranslation(): Observable<Record<string, string>> {
    return of({});
  }
}

export const TEST_PROVIDERS: Array<Provider | EnvironmentProviders> = [
  provideHttpClient(),
  provideHttpClientTesting(),

  provideRouter([]),

  provideNoopAnimations(),

  provideToastr({
    positionClass: 'toast-top-right',
    preventDuplicates: true,
    timeOut: 3000,
    progressBar: true,
    closeButton: true
  }),

  provideTranslateService({
    loader: {
      provide: TranslateLoader,
      useClass: FakeTranslateLoader
    },
    fallbackLang: 'en',
    lang: 'en'
  })
];

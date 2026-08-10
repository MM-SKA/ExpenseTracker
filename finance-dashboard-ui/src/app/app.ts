import { Component, computed, inject, signal } from '@angular/core';
import { AuthUser } from './shared/models/auth/auth-user';
import {
  NavigationEnd,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';
import { ScrollTopComponent } from './shared/components/scroll-top/scroll-top';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    ScrollTopComponent,
    TranslatePipe
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {

  protected readonly title = signal('finance-dashboard-ui');

  public readonly authUser = signal<AuthUser | null>(
    this.loadUser()
  );

  public readonly isAuthenticated = computed(
    () => !!this.authUser()
  );

  public readonly userInitial = computed(
    () =>
      this.authUser()?.fullName?.charAt(0).toUpperCase() ?? 'F'
  );

  public readonly router = inject(Router);

  private readonly translate = inject(TranslateService);

  currentLanguage =
    localStorage.getItem('language') ?? 'en';

  private readonly supportedLanguages = ['en', 'fr', 'ja'];

  constructor() {

    this.initializeLanguage();

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.authUser.set(this.loadUser());
      }
    });
  }

  private initializeLanguage(): void {

    const savedLanguage =
      localStorage.getItem('language');

    if (
      savedLanguage &&
      this.supportedLanguages.includes(savedLanguage)
    ) {

      this.currentLanguage = savedLanguage;

    } else {

      this.currentLanguage =
        this.detectBrowserLanguage();

    }

    this.translate.use(this.currentLanguage);
  }

  private detectBrowserLanguage(): string {

    for (const language of navigator.languages) {

      const shortLanguage =
        language.split('-')[0];

      if (
        this.supportedLanguages.includes(shortLanguage)
      ) {
        return shortLanguage;
      }
    }

    return 'en';
  }

  public changeLanguage(language: string): void {

    if (
      !this.supportedLanguages.includes(language)
    ) {
      return;
    }

    this.currentLanguage = language;

    // Save user's choice
    localStorage.setItem(
      'language',
      language
    );

    // Change UI language
    this.translate.use(language);
  }

  public signOut(): void {

    localStorage.removeItem('token');
    localStorage.removeItem('user');

    this.authUser.set(null);

    void this.router.navigateByUrl('/', {
      replaceUrl: true
    });
  }

  private loadUser(): AuthUser | null {

    if (typeof localStorage === 'undefined') {
      return null;
    }

    try {
      return JSON.parse(
        localStorage.getItem('user') || 'null'
      );
    } catch {
      return null;
    }
  }
}

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

  constructor() {

    this.translate.use(this.currentLanguage).subscribe({
      next: () => {
        console.log(
          `${this.currentLanguage} translations loaded`
        );
      },
      error: (error) => {
        console.error(
          'Translation loading failed:',
          error
        );
      }
    });

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.authUser.set(this.loadUser());
      }
    });
  }

  public changeLanguage(language: string): void {

    this.translate.use(language).subscribe({
      next: () => {

        this.currentLanguage = language;

        localStorage.setItem(
          'language',
          language
        );

        console.log(
          'Language changed to:',
          language
        );
      },

      error: (error) => {
        console.error(
          'Translation loading failed:',
          error
        );
      }
    });
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

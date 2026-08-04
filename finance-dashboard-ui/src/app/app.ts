import { Component, computed, inject, signal } from '@angular/core';
import { AuthUser } from './shared/models/auth/auth-user';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('finance-dashboard-ui');
  readonly authUser = signal<AuthUser | null>(this.loadUser());
  readonly isAuthenticated = computed(() => !!this.authUser());
  readonly userInitial = computed(() => this.authUser()?.fullName?.charAt(0).toUpperCase() ?? 'F');
  readonly router = inject(Router);

  constructor() {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.authUser.set(this.loadUser());
      }
    });
  }

  signOut(): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
    this.authUser.set(null);
    this.router.navigate(['/']);
  }

  private loadUser(): AuthUser | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }
    try {
      return JSON.parse(localStorage.getItem('user') || 'null');
    } catch {
      return null;
    }
  }
}

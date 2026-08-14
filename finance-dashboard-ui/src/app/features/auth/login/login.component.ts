import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryService } from '../../../core/services/category.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './login.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './login.css',
})
export class LoginComponent {
  email = '';

  password = '';

  errorMessage = '';

  readonly router = inject(Router);
  readonly authService = inject(AuthService);
  readonly categoryService = inject(CategoryService);
  public readonly translate = inject(TranslateService);

  login(): void {
    this.errorMessage = '';

    if (!this.isValidEmail(this.email)) {
      this.errorMessage = 'Invalid email format';
      return;
    }

    if (!this.password) {
      this.errorMessage = 'Password is required';
      return;
    }

    this.authService
      .login({
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: (response) => {
          localStorage.setItem(
            'user',
            JSON.stringify(response.data)
          );

          this.categoryService.getCategories(true).subscribe({
            next: () => this.router.navigate(['/expenses']),
            error: () => this.router.navigate(['/expenses']),
          });
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ??
            'Unable to login';

          console.error(error);
        },
      });
  }

  private isValidEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  }
}

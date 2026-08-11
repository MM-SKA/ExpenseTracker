import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryService } from '../../../core/services/category.service';
import { RegisterRequest } from '../../../shared/models/auth/register-request';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  fullName = '';
  email = '';
  password = '';
  phoneNumber = '';

  readonly router = inject(Router);
  readonly authService = inject(AuthService);
  readonly categoryService = inject(CategoryService);
  public readonly translate = inject(TranslateService);

  register(): void {
    const request: RegisterRequest = {
      fullName: this.fullName,
      email: this.email,
      password: this.password,
      phoneNumber: this.phoneNumber,
    };

    this.authService.register(request).subscribe({
      next: (response) => {
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('user', JSON.stringify(response.data.user));
        this.categoryService.getCategories(true).subscribe({
          next: () => this.router.navigate(['/expenses']),
          error: () => this.router.navigate(['/expenses'])
        });
      },
      error: (error) => console.error(error)
    });
  }
}

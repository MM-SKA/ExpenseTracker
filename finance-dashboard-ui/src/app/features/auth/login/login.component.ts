import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {

  email = '';

  password = '';
  readonly router = inject(Router);
  readonly authService =
    inject(AuthService);
  readonly categoryService = inject(CategoryService);

  login(): void {

    this.authService.login({

      email: this.email,

      password: this.password

    }).subscribe({

      next: (response) => {

        // localStorage.setItem(
        //   'token',
        //   response.data.token);

        localStorage.setItem(
          'user',
          JSON.stringify(response.data.user));

        this.categoryService.getCategories(true).subscribe({
          next: () => this.router.navigate(['/expenses']),
          error: () => this.router.navigate(['/expenses'])
        });
      },

      error: (error) => {

        console.error(error);

      }
    });
  }
}

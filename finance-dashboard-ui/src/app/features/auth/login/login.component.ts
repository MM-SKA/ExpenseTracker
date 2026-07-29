import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {

  email = '';

  password = '';

  private authService =
    inject(AuthService);

  login(): void {

    this.authService.login({

      email: this.email,

      password: this.password

    }).subscribe({

      next: (response) => {

        localStorage.setItem(
          'token',
          response.token);

        localStorage.setItem(
          'user',
          JSON.stringify(response.user));

        console.log(response);
      },

      error: (error) => {

        console.error(error);

      }
    });
  }
}

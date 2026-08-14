import {
  Component,
  inject,
  ChangeDetectionStrategy
} from '@angular/core';

import {
  FormsModule
} from '@angular/forms';

import {
  Router,
  RouterLink
} from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth.service';

import {
  RegisterRequest
} from '../../../shared/models/auth/register-request';

import {
  TranslatePipe,
  TranslateService
} from '@ngx-translate/core';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    TranslatePipe
  ],
  templateUrl: './register.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './register.css',
})
export class Register {

  fullName = '';
  email = '';
  password = '';
  phoneNumber = '';

  errorMessage = '';

  readonly router =
    inject(Router);

  readonly authService =
    inject(AuthService);

  public readonly translate =
    inject(TranslateService);

  register(): void {

    this.errorMessage = '';

    if (!this.fullName.trim()) {
      this.errorMessage = 'Full name is required';
      return;
    }

    if (!this.isValidEmail(this.email)) {
      this.errorMessage = 'Invalid email format';
      return;
    }

    if (!this.password) {
      this.errorMessage = 'Password is required';
      return;
    }

    if (!this.isValidPhoneNumber(this.phoneNumber)) {
      this.errorMessage = 'Phone number must contain exactly 10 digits';
      return;
    }

    const request: RegisterRequest = {
      fullName: this.fullName.trim(),
      email: this.email.trim().toLowerCase(),
      password: this.password,
      phoneNumber: this.phoneNumber.trim(),
    };

    this.authService
      .register(request)
      .subscribe({
        next: () => {

          void this.router.navigate([
            '/'
          ]);

        },
        error: error => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to register';

          console.error(error);

        },
      });
  }

  private isValidEmail(email: string): boolean {

    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      .test(email.trim());

  }

  private isValidPhoneNumber(phoneNumber: string): boolean {

    return /^[0-9]{10}$/
      .test(phoneNumber.trim());

  }

}

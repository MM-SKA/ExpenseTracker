import { Component, inject, ChangeDetectionStrategy } from '@angular/core';

import { FormsModule } from '@angular/forms';

import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

import { RegisterRequest } from '../../../shared/models/auth/register-request';

import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
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

  readonly router = inject(Router);

  readonly authService = inject(AuthService);

  public readonly translate = inject(TranslateService);

  private readonly toastr = inject(ToastrService);

  register(): void {
    this.errorMessage = '';

    if (!this.fullName.trim()) {
      this.toastr.warning('Full name is required', 'Validation');
      return;
    }

    if (!this.isValidEmail(this.email)) {
      this.toastr.warning('Email is required', 'Validation');
      return;
    }

    if (!this.password.trim()) {
      this.toastr.warning('Password is required', 'Validation');
      return;
    }

    if (!this.phoneNumber.trim()) {
      this.toastr.warning('Phone number is required', 'Validation');
      return;
    }

    if (!this.isValidPhoneNumber(this.phoneNumber)) {
      this.toastr.warning('Phone number must contain exactly 10 digits', 'Validation');
      return;
    }

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/;

    if (!passwordRegex.test(this.password)) {
      this.toastr.warning(
        'Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number and one special character',
        'Weak Password',
      );

      return;
    }

    const request: RegisterRequest = {
      fullName: this.fullName.trim(),
      email: this.email.trim().toLowerCase(),
      password: this.password.trim(),
      phoneNumber: this.phoneNumber.trim(),
    };

    this.authService.register(request).subscribe({
      next: () => {
        void this.router.navigate(['/']);
      },
      error: (error) => {
        this.errorMessage = error?.error?.message ?? 'Unable to register';

        if (this.errorMessage.includes('Email already exists')) {
          this.toastr.error('An account with this email already exists', 'Duplicate Email');
          return;
        }

        if (this.errorMessage.includes('Phone number already exists')) {
          this.toastr.error('This phone number is already registered', 'Duplicate Phone Number');
          return;
        }

        console.error(this.errorMessage);
      },
    });
  }

  private isValidEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }

  private isValidPhoneNumber(phoneNumber: string): boolean {
    return /^[0-9]{10}$/.test(phoneNumber.trim());
  }
}

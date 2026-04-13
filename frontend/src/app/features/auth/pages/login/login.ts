import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage = '';
  successMessage = '';

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.loginForm.getRawValue();

    this.authService
      .login({
        email: formValue.email ?? '',
        password: formValue.password ?? '',
      })
      .subscribe({
        next: (response) => {
          console.log('Login response:', response);

          if (response.token) {
            this.authService.saveToken(response.token);
            this.successMessage = 'Successful login!';
            this.errorMessage = '';

            this.router.navigate(['/profile']);
          }
        },
        error: (error) => {
          console.error('Login error:', error);
          this.successMessage = '';
          this.errorMessage = 'Failed to login. Please check your email and password.';
        },
      });
  }
}

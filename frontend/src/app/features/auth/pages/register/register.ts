import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  errorMessage = '';
  successMessage = '';

  registerForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[A-Z])(?=.*\d).+$/)],
    ],
  });

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.registerForm.getRawValue();

    this.authService
      .register({
        name: formValue.name ?? '',
        email: formValue.email ?? '',
        password: formValue.password ?? '',
      })
      .subscribe({
        next: (response) => {
          console.log('Register response:', response);
          this.successMessage = 'Registration successful!';
          this.errorMessage = '';
        },
        error: (error) => {
          console.error('Register error:', error);
          this.errorMessage = error?.error || 'Failed to register.';
          this.successMessage = '';
        },
      });
  }
}

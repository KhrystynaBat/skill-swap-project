import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProfileService } from '../../../../core/services/profile.service';
import { ProfileResponse } from '../../../../models/profile.model';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.scss',
})
export class EditProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  profile: ProfileResponse | null = null;

  editProfileForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    bio: [''],
    city: [''],
    avatarUrl: [''],
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.profileService.getMyProfile().subscribe({
      next: (response) => {
        this.profile = response;

        this.editProfileForm.patchValue({
          name: response.user.name ?? '',
          bio: response.user.bio ?? '',
          city: response.user.city ?? '',
          avatarUrl: response.user.avatarUrl ?? '',
        });

        this.isLoading = false;
      },
      error: (error) => {
        console.error('Load profile error:', error);
        this.errorMessage = 'Failed to load profile data.';
        this.isLoading = false;
      },
    });
  }

  onSubmit(): void {
    if (this.editProfileForm.invalid) {
      this.editProfileForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.editProfileForm.getRawValue();

    this.profileService
      .updateMyProfile({
        name: formValue.name ?? '',
        bio: formValue.bio ?? '',
        city: formValue.city ?? '',
        avatarUrl: formValue.avatarUrl ?? '',
      })
      .subscribe({
        next: (response) => {
          console.log('Update profile response:', response);
          this.successMessage = 'Profile updated successfully.';
          this.isSaving = false;

          setTimeout(() => {
            this.router.navigate(['/profile']);
          }, 700);
        },
        error: (error) => {
          console.error('Update profile error:', error);
          this.errorMessage = error?.error || 'Failed to update profile.';
          this.isSaving = false;
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/profile']);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    const reader = new FileReader();

    reader.onload = () => {
      const result = reader.result as string;

      this.editProfileForm.patchValue({
        avatarUrl: result,
      });
    };

    reader.readAsDataURL(file);
  }
}

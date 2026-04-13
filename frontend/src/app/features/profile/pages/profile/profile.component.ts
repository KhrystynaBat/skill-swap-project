import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {
  ProfileService,
  UserSkillItem,
  UserInterestItem,
} from '../../../../core/services/profile.service';
import { ProfileResponse } from '../../../../models/profile.model';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { SkillsService } from '../../../../core/services/skills.service';
import { Skill } from '../../../../models/skill.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  private profileService = inject(ProfileService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private skillsService = inject(SkillsService);

  availableSkills: Skill[] = [];

  profile: ProfileResponse | null = null;
  skills: UserSkillItem[] = [];
  interests: UserInterestItem[] = [];

  isLoading = true;
  errorMessage = '';

  isAddSkillModalOpen = false;
  isAddInterestModalOpen = false;

  skillMessage = '';
  interestMessage = '';

  addSkillForm = this.fb.group({
    skillId: [null as number | null, [Validators.required, Validators.min(1)]],
    level: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
  });

  addInterestForm = this.fb.group({
    skillId: [null as number | null, [Validators.required, Validators.min(1)]],
    priority: [1, [Validators.required, Validators.min(1), Validators.max(3)]],
  });

  ngOnInit(): void {
    this.loadAllProfileData();
    this.loadAvailableSkills();
  }

  loadAllProfileData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.profileService.getMyProfile().subscribe({
      next: (response) => {
        this.profile = response;
        this.loadSkills();
        this.loadInterests();
      },
      error: (error) => {
        console.error('Profile error:', error);
        this.errorMessage = 'Failed to load profile.';
        this.isLoading = false;
      },
    });
  }

  loadSkills(): void {
    this.profileService.getMySkills().subscribe({
      next: (response) => {
        this.skills = response;
        this.finishLoadingIfReady();
      },
      error: (error) => {
        console.error('Skills error:', error);
        this.finishLoadingIfReady();
      },
    });
  }

  loadInterests(): void {
    this.profileService.getMyInterests().subscribe({
      next: (response) => {
        this.interests = response;
        this.finishLoadingIfReady();
      },
      error: (error) => {
        console.error('Interests error:', error);
        this.finishLoadingIfReady();
      },
    });
  }

  loadAvailableSkills(): void {
    this.skillsService.getSkills().subscribe({
      next: (response) => {
        console.log('Available skills:', response);
        this.availableSkills = response;
      },
      error: (error) => {
        console.error('Available skills error:', error);
      },
    });
  }

  finishLoadingIfReady(): void {
    this.isLoading = false;
  }

  goToEditProfile(): void {
    this.router.navigate(['/profile/edit']);
  }

  openAddSkillModal(): void {
    this.skillMessage = '';
    this.addSkillForm.reset({
      skillId: null,
      level: 1,
    });
    this.isAddSkillModalOpen = true;
  }

  closeAddSkillModal(): void {
    this.isAddSkillModalOpen = false;
  }

  openAddInterestModal(): void {
    this.interestMessage = '';
    this.addInterestForm.reset({
      skillId: null,
      priority: 1,
    });
    this.isAddInterestModalOpen = true;
  }

  closeAddInterestModal(): void {
    this.isAddInterestModalOpen = false;
  }

  submitAddSkill(): void {
    if (this.addSkillForm.invalid) {
      this.addSkillForm.markAllAsTouched();
      return;
    }

    const formValue = this.addSkillForm.getRawValue();

    this.profileService
      .addSkill({
        skillId: formValue.skillId ?? 0,
        level: formValue.level ?? 1,
      })
      .subscribe({
        next: (response) => {
          console.log('Add skill response:', response);
          this.skillMessage = 'Skill added.';
          this.loadSkills();

          setTimeout(() => {
            this.closeAddSkillModal();
          }, 500);
        },
        error: (error) => {
          console.error('Add skill error:', error);
          this.skillMessage = error?.error || 'Failed to add skill.';
        },
      });
  }

  submitAddInterest(): void {
    if (this.addInterestForm.invalid) {
      this.addInterestForm.markAllAsTouched();
      return;
    }

    const formValue = this.addInterestForm.getRawValue();

    this.profileService
      .addInterest({
        skillId: formValue.skillId ?? 0,
        priority: formValue.priority ?? 1,
      })
      .subscribe({
        next: (response) => {
          console.log('Add interest response:', response);
          this.interestMessage = 'Interest added.';
          this.loadInterests();

          setTimeout(() => {
            this.closeAddInterestModal();
          }, 500);
        },
        error: (error) => {
          console.error('Add interest error:', error);
          this.interestMessage = error?.error || 'Failed to add interest.';
        },
      });
  }

  getLevelLabel(level: number): string {
    switch (level) {
      case 1:
        return 'Beginner';
      case 2:
        return 'Elementary';
      case 3:
        return 'Intermediate';
      case 4:
        return 'Advanced';
      case 5:
        return 'Expert';
      default:
        return '';
    }
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 1:
        return 'Low interest';
      case 2:
        return 'Medium interest';
      case 3:
        return 'High interest';
      default:
        return '';
    }
  }
}

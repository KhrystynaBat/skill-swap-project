import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

import { Skill } from '../../../../models/skill.model';
import { SearchUser } from '../../../../models/search-user.model';

import { SkillsService } from '../../../../core/services/skills.service';
import { UsersService } from '../../../../core/services/users.service';

import { RouterModule, Routes } from '@angular/router';

@Component({
  selector: 'app-search-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './search-users.component.html',
  styleUrl: './search-users.component.scss',
})
export class SearchUsersComponent implements OnInit {
  private fb = inject(FormBuilder);
  private skillsService = inject(SkillsService);
  private usersService = inject(UsersService);

  availableSkills: Skill[] = [];
  filteredSkills: Skill[] = [];
  categories: string[] = [];

  selectedSkill: Skill | null = null;
  foundUsers: SearchUser[] = [];

  isLoadingSkills = true;
  isSearching = false;
  errorMessage = '';
  searchMessage = '';

  searchForm = this.fb.group({
    query: [''],
    category: [''],
    city: [''],
  });

  ngOnInit(): void {
    this.loadSkills();
    this.loadInitialUsers();

    this.searchForm.valueChanges.subscribe(() => {
      this.filterSkills();
    });
  }

  loadSkills(): void {
    this.skillsService.getSkills().subscribe({
      next: (response) => {
        this.availableSkills = response;
        this.filteredSkills = response;
        this.categories = [...new Set(response.map((skill) => skill.category))].sort();
        this.isLoadingSkills = false;
      },
      error: (error) => {
        console.error('Load skills error:', error);
        this.errorMessage = 'Failed to load skills.';
        this.isLoadingSkills = false;
      },
    });
  }

  loadInitialUsers(): void {
    this.isSearching = true;
    this.errorMessage = '';
    this.searchMessage = '';

    this.usersService.searchUsers().subscribe({
      next: (response) => {
        this.foundUsers = response;
        this.isSearching = false;

        if (response.length === 0) {
          this.searchMessage = 'No users found.';
        }
      },
      error: (error) => {
        console.error('Initial users load error:', error);
        this.errorMessage = 'Failed to load users.';
        this.isSearching = false;
      },
    });
  }

  filterSkills(): void {
    const query = (this.searchForm.value.query ?? '').trim().toLowerCase();
    const category = this.searchForm.value.category ?? '';

    this.filteredSkills = this.availableSkills.filter((skill) => {
      const matchesQuery =
        !query ||
        skill.name.toLowerCase().includes(query) ||
        skill.category.toLowerCase().includes(query);

      const matchesCategory = !category || skill.category === category;

      return matchesQuery && matchesCategory;
    });

    if (
      this.selectedSkill &&
      !this.filteredSkills.some((skill) => skill.id === this.selectedSkill?.id)
    ) {
      this.selectedSkill = null;
    }
  }

  selectSkill(skill: Skill): void {
    this.selectedSkill = skill;
    this.searchForm.patchValue(
      {
        query: skill.name,
      },
      { emitEvent: false },
    );
    this.filteredSkills = [skill];
  }

  clearSelectedSkill(): void {
    this.selectedSkill = null;
    this.searchForm.patchValue({
      query: '',
    });
    this.filterSkills();
  }

  onSubmit(): void {
    const city = this.searchForm.value.city ?? '';
    const category = this.searchForm.value.category ?? '';

    this.isSearching = true;
    this.errorMessage = '';
    this.searchMessage = '';
    this.foundUsers = [];

    this.usersService.searchUsers(this.selectedSkill?.id, city, category).subscribe({
      next: (response) => {
        this.foundUsers = response;
        this.isSearching = false;

        if (response.length === 0) {
          this.searchMessage = 'No users found for your request.';
        }
      },
      error: (error) => {
        console.error('Search users error:', error);
        this.errorMessage = 'Failed to search users.';
        this.isSearching = false;
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
        return `Level ${level}`;
    }
  }

  get shouldShowSkillsDropdown(): boolean {
    const query = (this.searchForm.value.query ?? '').trim();
    return query.length >= 2 && !this.selectedSkill && this.filteredSkills.length > 0;
  }

  isMatchingSkill(skillName: string): boolean {
    return this.selectedSkill?.name.toLowerCase() === skillName.toLowerCase();
  }
}

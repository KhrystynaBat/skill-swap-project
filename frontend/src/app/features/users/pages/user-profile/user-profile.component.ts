import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UsersService } from '../../../../core/services/users.service';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss',
})
export class UserProfileComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private usersService = inject(UsersService);

  user: any = null;
  reviews: any[] = [];
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.usersService.getUserById(id).subscribe({
      next: (res) => {
        this.user = res;
        this.isLoading = false;
        this.loadReviews(id);
      },
      error: (error) => {
        console.error('Load user profile error:', error);
        this.errorMessage = 'Failed to load user profile.';
        this.isLoading = false;
      },
    });
  }

  createMatch(userId: number) {
    this.usersService.createMatch(userId).subscribe({
      next: () => {
        this.router.navigate(['/matches']);
      },
      error: (err) => {
        alert(this.getErrorMessage(err, 'Error'));
      },
    });
  }

  openChat(userId: number): void {
    this.router.navigate(['/chat', userId]);
  }

  private loadReviews(userId: number): void {
    this.usersService.getUserReviews(userId).subscribe({
      next: (reviews) => {
        this.reviews = reviews;
      },
      error: (error) => {
        console.error('Load reviews error:', error);
      },
    });
  }

  private getErrorMessage(error: any, fallback: string): string {
    if (typeof error?.error === 'string') {
      return error.error;
    }

    if (error?.error?.title) {
      return error.error.title;
    }

    return fallback;
  }
}

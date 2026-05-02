import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsersService } from '../../../../core/services/users.service';
import { AuthService } from '../../../../core/services/auth.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-matches',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './matches.component.html',
  styleUrl: './matches.component.scss',
})
export class MatchesComponent implements OnInit {
  private usersService = inject(UsersService);
  private authService = inject(AuthService);

  matches: any[] = [];
  isLoading = true;
  errorMessage = '';

  currentUserId: number | null = null;
  usersMap: { [id: number]: any } = {};
  loadingMatchId: number | null = null;
  reviewMatchId: number | null = null;
  reviewRating = 5;
  reviewComment = '';
  reviewMessage = '';

  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId();

    console.log('Current user:', this.currentUserId);

    this.loadMatches();
  }

  loadMatches(): void {
    this.usersService.getMyMatches().subscribe({
      next: (res) => {
        this.matches = res;

        console.log('Matches:', this.matches);

        this.isLoading = false;

        this.loadUsers();
      },
      error: () => {
        this.errorMessage = 'Failed to load matches';
        this.isLoading = false;
      },
    });
  }

  loadUsers() {
    this.matches.forEach((match) => {
      const otherId = this.getOtherUserId(match);

      if (!this.usersMap[otherId]) {
        this.usersService.getUserById(otherId).subscribe((user) => {
          this.usersMap[otherId] = user;
        });
      }
    });
  }

  acceptMatch(match: any) {
    const matchId = match.id;
    const otherUserId = this.getOtherUserId(match);

    this.loadingMatchId = matchId;

    this.usersService.updateMatchStatus(matchId, 'active').subscribe({
      next: () => {
        match.status = 'active';
        this.loadingMatchId = null;
        this.loadMatches();
      },
      error: (error) => {
        this.errorMessage = this.getErrorMessage(error, 'Failed to accept match');
        this.loadingMatchId = null;
      },
    });
  }

  rejectMatch(matchId: number) {
    this.loadingMatchId = matchId;

    this.usersService.updateMatchStatus(matchId, 'rejected').subscribe({
      next: () => {
        this.loadMatches();
        this.loadingMatchId = null;
      },
      error: () => {
        this.loadingMatchId = null;
      },
    });
  }

  openFinishMatch(match: any): void {
    this.reviewMatchId = match.id;
    this.reviewRating = 5;
    this.reviewComment = '';
    this.reviewMessage = '';
  }

  cancelFinishMatch(): void {
    this.reviewMatchId = null;
    this.reviewComment = '';
    this.reviewMessage = '';
  }

  submitReview(match: any): void {
    const targetUserId = this.getOtherUserId(match);
    this.loadingMatchId = match.id;
    this.reviewMessage = '';

    this.usersService
      .createUserReview(targetUserId, this.reviewRating, this.reviewComment.trim())
      .subscribe({
        next: () => {
          match.status = 'completed';
          this.loadingMatchId = null;
          this.reviewMatchId = null;
          this.reviewComment = '';
          this.loadMatches();
        },
        error: (error) => {
          this.loadingMatchId = null;
          this.reviewMessage = this.getErrorMessage(error, 'Failed to finish match');
        },
      });
  }

  getOtherUserId(match: any): number {
    return match.userAId === this.currentUserId ? match.userBId : match.userAId;
  }

  isIncoming(match: any): boolean {
    return match.userBId === this.currentUserId;
  }

  isOutgoing(match: any): boolean {
    return match.userAId === this.currentUserId;
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

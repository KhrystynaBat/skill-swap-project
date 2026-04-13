import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersService } from '../../../../core/services/users.service';
import { AuthService } from '../../../../core/services/auth.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-matches',
  standalone: true,
  imports: [CommonModule, RouterModule],
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

  acceptMatch(matchId: number) {
    this.loadingMatchId = matchId;

    this.usersService.updateMatchStatus(matchId, 'active').subscribe({
      next: () => {
        this.loadMatches();
        this.loadingMatchId = null;
      },
      error: () => {
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

  getOtherUserId(match: any): number {
    return match.userAId === this.currentUserId ? match.userBId : match.userAId;
  }

  isIncoming(match: any): boolean {
    return match.userBId === this.currentUserId;
  }

  isOutgoing(match: any): boolean {
    return match.userAId === this.currentUserId;
  }
}

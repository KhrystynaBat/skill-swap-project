import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
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
  private usersService = inject(UsersService);

  user: any = null;
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.usersService.getUserById(id).subscribe({
      next: (res) => {
        this.user = res;
        this.isLoading = false;
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
        alert('Match request sent');
      },
      error: (err) => {
        alert(err.error || 'Error');
      },
    });
  }
}

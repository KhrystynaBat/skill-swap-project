import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SearchUser } from '../../models/search-user.model';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  searchUsers(skillId?: number, city?: string, category?: string): Observable<SearchUser[]> {
    let params = new HttpParams();

    if (skillId && skillId > 0) {
      params = params.set('skillId', skillId);
    }

    if (city && city.trim()) {
      params = params.set('city', city.trim());
    }

    if (category && category.trim()) {
      params = params.set('category', category.trim());
    }

    return this.http.get<SearchUser[]>(`${this.apiUrl}/users/search`, {
      params,
    });
  }

  getUserById(userId: number) {
    return this.http.get<any>(`${this.apiUrl}/users/${userId}`);
  }

  getMyMatches() {
    return this.http.get<any[]>(`${this.apiUrl}/matches/my`);
  }

  updateMatchStatus(matchId: number, status: string) {
    return this.http.put(`${this.apiUrl}/match/${matchId}/status?status=${status}`, {}, {
      responseType: 'text',
    });
  }

  createMatch(targetUserId: number) {
    return this.http.post(`${this.apiUrl}/match/${targetUserId}`, {}, {
      responseType: 'text',
    });
  }

  getUserReviews(userId: number) {
    return this.http.get<any[]>(`${this.apiUrl}/review/user/${userId}`);
  }

  createUserReview(userId: number, rating: number, comment: string) {
    return this.http.post(
      `${this.apiUrl}/review/user/${userId}`,
      { rating, comment },
      { responseType: 'text' },
    );
  }
}

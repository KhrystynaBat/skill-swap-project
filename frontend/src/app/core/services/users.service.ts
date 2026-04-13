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

    return this.http.get<SearchUser[]>(`${this.apiUrl}/api/users/search`, {
      params,
    });
  }

  getUserById(userId: number) {
    return this.http.get<any>(`${this.apiUrl}/api/users/${userId}`);
  }

  getMyMatches() {
    return this.http.get<any[]>('http://localhost:5194/api/match/my');
  }

  updateMatchStatus(matchId: number, status: string) {
    return this.http.put(`http://localhost:5194/api/match/${matchId}/status?status=${status}`, {});
  }

  createMatch(targetUserId: number) {
    return this.http.post(`http://localhost:5194/api/match/${targetUserId}`, {});
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProfileResponse } from '../../models/profile.model';

export interface UpdateProfileRequest {
  name?: string | null;
  bio?: string | null;
  city?: string | null;
  avatarUrl?: string | null;
}

export interface UserSkillItem {
  id: number;
  name: string;
  category: string;
  level: number;
}

export interface UserInterestItem {
  id: number;
  name: string;
  category: string;
  priority: number;
}

export interface AddSkillRequest {
  skillId: number;
  level: number;
}

export interface AddInterestRequest {
  skillId: number;
  priority: number;
}

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getMyProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiUrl}/api/profile/me`);
  }

  updateMyProfile(data: UpdateProfileRequest): Observable<string> {
    return this.http.put(`${this.apiUrl}/api/profile/me`, data, {
      responseType: 'text',
    });
  }

  getMySkills(): Observable<UserSkillItem[]> {
    return this.http.get<UserSkillItem[]>(`${this.apiUrl}/api/profile/skills`);
  }

  addSkill(data: AddSkillRequest): Observable<string> {
    return this.http.post(`${this.apiUrl}/api/profile/skills`, data, {
      responseType: 'text',
    });
  }

  getMyInterests(): Observable<UserInterestItem[]> {
    return this.http.get<UserInterestItem[]>(`${this.apiUrl}/api/profile/interests`);
  }

  addInterest(data: AddInterestRequest): Observable<string> {
    return this.http.post(`${this.apiUrl}/api/profile/interests`, data, {
      responseType: 'text',
    });
  }
}

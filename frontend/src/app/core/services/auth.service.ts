import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RegisterRequest } from '../../models/register-request.model';
import { LoginRequest } from '../../models/login-request.model';
import { AuthResponse } from '../../models/auth-response.model';
import { AuthMeResponse } from '../../models/auth-me.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  private tokenKey = 'skill_swap_token';

  register(data: RegisterRequest): Observable<string> {
    return this.http.post(`${this.apiUrl}/auth/register`, data, {
      responseType: 'text',
    });
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, data);
  }

  getMe(): Observable<AuthMeResponse> {
    return this.http.get<AuthMeResponse>(`${this.apiUrl}/auth/me`);
  }

  saveToken(token: string): void {
    sessionStorage.setItem(this.tokenKey, token);
    localStorage.removeItem(this.tokenKey);
  }

  getToken(): string | null {
    const sessionToken = sessionStorage.getItem(this.tokenKey);

    if (sessionToken) {
      return sessionToken;
    }

    if (localStorage.getItem(this.tokenKey)) {
      localStorage.removeItem(this.tokenKey);
    }

    return null;
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserId(): number | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      const id =
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
        payload.nameid ||
        payload.sub;

      return id ? Number(id) : null;
    } catch {
      return null;
    }
  }
}

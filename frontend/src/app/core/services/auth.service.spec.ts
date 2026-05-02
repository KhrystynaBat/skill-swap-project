import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  const makeToken = (payload: Record<string, unknown>) =>
    `header.${btoa(JSON.stringify(payload))}.signature`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    sessionStorage.clear();
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
    localStorage.clear();
  });

  it('posts register data to the auth endpoint', () => {
    service.register({ name: 'Olesia', email: 'olesia@test.com', password: 'Pass123' }).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/register`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      name: 'Olesia',
      email: 'olesia@test.com',
      password: 'Pass123',
    });
    expect(req.request.responseType).toBe('text');
    req.flush('User registered');
  });

  it('posts login data to the auth endpoint', () => {
    service.login({ email: 'user@test.com', password: 'Pass123' }).subscribe((response) => {
      expect(response.token).toBe('jwt');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({ token: 'jwt' });
  });

  it('loads current user information', () => {
    service.getMe().subscribe((response) => {
      expect(response.userId).toBe('7');
      expect(response.email).toBe('user@test.com');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/me`);
    expect(req.request.method).toBe('GET');
    req.flush({ userId: '7', email: 'user@test.com' });
  });

  it('saves token in session storage', () => {
    service.saveToken('token-1');

    expect(sessionStorage.getItem('skill_swap_token')).toBe('token-1');
  });

  it('reports logged in when token exists', () => {
    sessionStorage.setItem('skill_swap_token', 'token');

    expect(service.isLoggedIn()).toBeTrue();
  });

  it('parses user id from framework claim', () => {
    service.saveToken(
      makeToken({
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': '42',
      }),
    );

    expect(service.getUserId()).toBe(42);
  });

  it('returns null for invalid token payload', () => {
    service.saveToken('not.a.valid.token');

    expect(service.getUserId()).toBeNull();
  });
});

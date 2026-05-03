import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UsersService } from './users.service';
import { environment } from '../../../environments/environment';

describe('UsersService', () => {
  let service: UsersService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(UsersService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('searches users without filters', () => {
    service.searchUsers().subscribe((users) => expect(users.length).toBe(0));

    const req = httpMock.expectOne(`${environment.apiUrl}/users/search`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.keys().length).toBe(0);
    req.flush([]);
  });

  it('adds skill filter to search request', () => {
    service.searchUsers(3).subscribe();

    const req = httpMock.expectOne((request) => request.url.endsWith('/users/search'));
    expect(req.request.params.get('skillId')).toBe('3');
    req.flush([]);
  });

  it('adds city filter to search request', () => {
    service.searchUsers(undefined, ' Lviv ').subscribe();

    const req = httpMock.expectOne((request) => request.url.endsWith('/users/search'));
    expect(req.request.params.get('city')).toBe('Lviv');
    req.flush([]);
  });

  it('adds category filter to search request', () => {
    service.searchUsers(undefined, undefined, 'IT').subscribe();

    const req = httpMock.expectOne((request) => request.url.endsWith('/users/search'));
    expect(req.request.params.get('category')).toBe('IT');
    req.flush([]);
  });

  it('loads user by id', () => {
    service.getUserById(4).subscribe((user) => expect(user.id).toBe(4));

    const req = httpMock.expectOne(`${environment.apiUrl}/users/4`);
    expect(req.request.method).toBe('GET');
    req.flush({ id: 4 });
  });

  it('loads current user matches', () => {
    service.getMyMatches().subscribe((matches) => expect(matches.length).toBe(1));

    const req = httpMock.expectOne(`${environment.apiUrl}/match/my`);
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 1 }]);
  });

  it('updates match status as text response', () => {
    service.updateMatchStatus(5, 'active').subscribe((response) => expect(response).toBe('Match updated'));

    const req = httpMock.expectOne(`${environment.apiUrl}/match/5/status?status=active`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.responseType).toBe('text');
    req.flush('Match updated');
  });

  it('creates match as text response', () => {
    service.createMatch(8).subscribe((response) => expect(response).toBe('Match created'));

    const req = httpMock.expectOne(`${environment.apiUrl}/match/8`);
    expect(req.request.method).toBe('POST');
    expect(req.request.responseType).toBe('text');
    req.flush('Match created');
  });

  it('loads user reviews', () => {
    service.getUserReviews(2).subscribe((reviews) => expect(reviews.length).toBe(1));

    const req = httpMock.expectOne(`${environment.apiUrl}/review/user/2`);
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 1, rating: 5 }]);
  });

  it('creates user review as text response', () => {
    service.createUserReview(2, 5, 'Great job').subscribe((response) => {
      expect(response).toBe('Review created');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/review/user/2`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ rating: 5, comment: 'Great job' });
    expect(req.request.responseType).toBe('text');
    req.flush('Review created');
  });
});

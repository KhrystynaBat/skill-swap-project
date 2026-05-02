import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ProfileService } from './profile.service';
import { environment } from '../../../environments/environment';

describe('ProfileService', () => {
  let service: ProfileService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ProfileService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loads my profile', () => {
    service.getMyProfile().subscribe((profile) => expect(profile.user.name).toBe('Olesia'));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/profile/me`);
    expect(req.request.method).toBe('GET');
    req.flush({ user: { name: 'Olesia' }, rating: { average: 5, count: 1 } });
  });

  it('updates my profile as text response', () => {
    service.updateMyProfile({ name: 'Updated' }).subscribe((response) => expect(response).toBe('Profile updated'));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/profile/me`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ name: 'Updated' });
    expect(req.request.responseType).toBe('text');
    req.flush('Profile updated');
  });

  it('loads my skills', () => {
    service.getMySkills().subscribe((skills) => expect(skills[0].name).toBe('C#'));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/profile/skills`);
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 1, name: 'C#', category: 'IT', level: 4 }]);
  });

  it('adds skill as text response', () => {
    service.addSkill({ skillId: 1, level: 5 }).subscribe((response) => expect(response).toBe('Skill added'));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/profile/skills`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ skillId: 1, level: 5 });
    expect(req.request.responseType).toBe('text');
    req.flush('Skill added');
  });

  it('loads my interests', () => {
    service.getMyInterests().subscribe((items) => expect(items[0].name).toBe('Photography'));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/profile/interests`);
    expect(req.request.method).toBe('GET');
    req.flush([{ id: 2, name: 'Photography', category: 'Art', priority: 3 }]);
  });

  it('adds interest as text response', () => {
    service.addInterest({ skillId: 2, priority: 3 }).subscribe((response) => expect(response).toBe('Interest added'));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/profile/interests`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ skillId: 2, priority: 3 });
    expect(req.request.responseType).toBe('text');
    req.flush('Interest added');
  });
});

import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { SkillsService } from './skills.service';
import { environment } from '../../../environments/environment';

describe('SkillsService', () => {
  let service: SkillsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(SkillsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loads skills from API', () => {
    service.getSkills().subscribe((skills) => expect(skills.length).toBe(2));

    const req = httpMock.expectOne(`${environment.apiUrl}/api/skills`);
    expect(req.request.method).toBe('GET');
    req.flush([
      { id: 1, name: 'C#', category: 'IT' },
      { id: 2, name: 'Photoshop', category: 'Design' },
    ]);
  });
});

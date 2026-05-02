import { TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { SkillsService } from '../../../../core/services/skills.service';
import { UsersService } from '../../../../core/services/users.service';
import { SearchUsersComponent } from './search-users.component';

describe('SearchUsersComponent', () => {
  let component: SearchUsersComponent;
  let skillsService: jasmine.SpyObj<SkillsService>;
  let usersService: jasmine.SpyObj<UsersService>;
  let router: jasmine.SpyObj<Router>;

  const skills = [
    { id: 1, name: 'C#', category: 'IT' },
    { id: 2, name: 'Photography', category: 'Art' },
    { id: 3, name: 'Photoshop', category: 'Design' },
  ];

  beforeEach(() => {
    skillsService = jasmine.createSpyObj<SkillsService>('SkillsService', ['getSkills']);
    usersService = jasmine.createSpyObj<UsersService>('UsersService', ['searchUsers', 'createMatch']);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    skillsService.getSkills.and.returnValue(of(skills));
    usersService.searchUsers.and.returnValue(of([]));

    TestBed.configureTestingModule({
      providers: [
        FormBuilder,
        { provide: SkillsService, useValue: skillsService },
        { provide: UsersService, useValue: usersService },
        { provide: Router, useValue: router },
      ],
    });

    component = TestBed.runInInjectionContext(() => new SearchUsersComponent());
  });

  it('loads skills and categories on init', () => {
    component.ngOnInit();

    expect(component.availableSkills.length).toBe(3);
    expect(component.categories).toEqual(['Art', 'Design', 'IT']);
  });

  it('filters skills by query', () => {
    component.availableSkills = skills;
    component.searchForm.patchValue({ query: 'photo', category: '', city: '' });

    component.filterSkills();

    expect(component.filteredSkills.map((skill) => skill.name)).toEqual(['Photography', 'Photoshop']);
  });

  it('selects skill and updates query', () => {
    component.selectSkill(skills[1]);

    expect(component.selectedSkill).toEqual(skills[1]);
    expect(component.searchForm.value.query).toBe('Photography');
  });

  it('submits search with selected skill and filters', () => {
    usersService.searchUsers.and.returnValue(of([{ id: 10, name: 'Vasyl', teachSkills: [], learnSkills: [] } as any]));
    component.selectedSkill = skills[0];
    component.searchForm.patchValue({ city: 'Lviv', category: 'IT' });

    component.onSubmit();

    expect(usersService.searchUsers).toHaveBeenCalledWith(1, 'Lviv', 'IT');
    expect(component.foundUsers.length).toBe(1);
  });

  it('shows message when search returns empty result', () => {
    usersService.searchUsers.and.returnValue(of([]));

    component.onSubmit();

    expect(component.searchMessage).toBe('No users found for your request.');
  });

  it('shows error when search fails', () => {
    usersService.searchUsers.and.returnValue(throwError(() => ({ status: 500 })));

    component.onSubmit();

    expect(component.errorMessage).toBe('Failed to search users.');
  });

  it('creates match and navigates to matches', () => {
    usersService.createMatch.and.returnValue(of('Match created'));

    component.createMatch(4);

    expect(usersService.createMatch).toHaveBeenCalledWith(4);
    expect(router.navigate).toHaveBeenCalledWith(['/matches']);
  });

  it('shows backend match error', () => {
    usersService.createMatch.and.returnValue(
      throwError(() => ({ error: 'Your skills and interests do not match' })),
    );

    component.createMatch(4);

    expect(component.errorMessage).toBe('Your skills and interests do not match');
  });

});

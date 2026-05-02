import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { UsersService } from '../../../../core/services/users.service';
import { MatchesComponent } from './matches.component';

describe('MatchesComponent', () => {
  let component: MatchesComponent;
  let usersService: jasmine.SpyObj<UsersService>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    usersService = jasmine.createSpyObj<UsersService>('UsersService', [
      'getMyMatches',
      'getUserById',
      'updateMatchStatus',
      'createUserReview',
    ]);
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['getUserId']);
    authService.getUserId.and.returnValue(1);
    usersService.getMyMatches.and.returnValue(of([]));
    usersService.getUserById.and.returnValue(of({ id: 2, name: 'Khrystyna' }));

    TestBed.configureTestingModule({
      providers: [
        { provide: UsersService, useValue: usersService },
        { provide: AuthService, useValue: authService },
      ],
    });

    component = TestBed.runInInjectionContext(() => new MatchesComponent());
  });

  it('loads matches on init', () => {
    usersService.getMyMatches.and.returnValue(of([{ id: 1, userAId: 2, userBId: 1, status: 'pending' }]));

    component.ngOnInit();

    expect(component.currentUserId).toBe(1);
    expect(component.matches.length).toBe(1);
  });

  it('accepts match and updates local status', () => {
    const match = { id: 1, userAId: 2, userBId: 1, status: 'pending' };
    component.currentUserId = 1;
    usersService.updateMatchStatus.and.returnValue(of('Match updated'));

    component.acceptMatch(match);

    expect(usersService.updateMatchStatus).toHaveBeenCalledWith(1, 'active');
    expect(match.status).toBe('active');
    expect(component.loadingMatchId).toBeNull();
  });

  it('shows accept error', () => {
    const match = { id: 1, userAId: 2, userBId: 1, status: 'pending' };
    component.currentUserId = 1;
    usersService.updateMatchStatus.and.returnValue(throwError(() => ({ error: 'Invalid status' })));

    component.acceptMatch(match);

    expect(component.errorMessage).toBe('Invalid status');
  });

  it('rejects match and reloads list', () => {
    usersService.updateMatchStatus.and.returnValue(of('Match updated'));

    component.rejectMatch(1);

    expect(usersService.updateMatchStatus).toHaveBeenCalledWith(1, 'rejected');
    expect(usersService.getMyMatches).toHaveBeenCalled();
  });

  it('opens finish match form', () => {
    component.openFinishMatch({ id: 3 });

    expect(component.reviewMatchId).toBe(3);
    expect(component.reviewRating).toBe(5);
    expect(component.reviewComment).toBe('');
  });

  it('submits review and completes match', () => {
    const match = { id: 3, userAId: 1, userBId: 2, status: 'active' };
    component.currentUserId = 1;
    component.reviewRating = 4;
    component.reviewComment = 'Good';
    usersService.createUserReview.and.returnValue(of('Review created'));

    component.submitReview(match);

    expect(usersService.createUserReview).toHaveBeenCalledWith(2, 4, 'Good');
    expect(match.status).toBe('completed');
    expect(component.reviewMatchId).toBeNull();
  });

  it('shows review error', () => {
    const match = { id: 3, userAId: 1, userBId: 2, status: 'active' };
    component.currentUserId = 1;
    usersService.createUserReview.and.returnValue(throwError(() => ({ error: 'Already reviewed' })));

    component.submitReview(match);

    expect(component.reviewMessage).toBe('Already reviewed');
  });

  it('calculates other user id', () => {
    component.currentUserId = 1;

    expect(component.getOtherUserId({ userAId: 1, userBId: 2 })).toBe(2);
    expect(component.getOtherUserId({ userAId: 3, userBId: 1 })).toBe(3);
  });

});

import { TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { LoginComponent } from './login';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['login', 'saveToken']);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        FormBuilder,
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
    });

    component = TestBed.runInInjectionContext(() => new LoginComponent());
  });

  it('does not submit invalid form', () => {
    component.onSubmit();

    expect(authService.login).not.toHaveBeenCalled();
  });

  it('logs in valid user and navigates to profile', () => {
    authService.login.and.returnValue(of({ token: 'jwt-token' }));
    component.loginForm.setValue({ email: 'user@test.com', password: 'Pass123' });

    component.onSubmit();

    expect(authService.login).toHaveBeenCalledWith({
      email: 'user@test.com',
      password: 'Pass123',
    });
    expect(authService.saveToken).toHaveBeenCalledWith('jwt-token');
    expect(router.navigate).toHaveBeenCalledWith(['/profile']);
  });

  it('shows login error when API fails', () => {
    authService.login.and.returnValue(throwError(() => ({ status: 401 })));
    component.loginForm.setValue({ email: 'user@test.com', password: 'Pass123' });

    component.onSubmit();

    expect(component.errorMessage).toContain('Failed to login');
  });

});

import { TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { RegisterComponent } from './register';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['register', 'login', 'saveToken']);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        FormBuilder,
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
    });

    component = TestBed.runInInjectionContext(() => new RegisterComponent());
  });

  it('does not submit invalid form', () => {
    component.onSubmit();

    expect(authService.register).not.toHaveBeenCalled();
  });

  it('registers, logs in and navigates to profile', () => {
    authService.register.and.returnValue(of('User registered'));
    authService.login.and.returnValue(of({ token: 'new-token' }));
    component.registerForm.setValue({
      name: 'New User',
      email: 'new@test.com',
      password: 'Pass123',
    });

    component.onSubmit();

    expect(authService.register).toHaveBeenCalledWith({
      name: 'New User',
      email: 'new@test.com',
      password: 'Pass123',
    });
    expect(authService.login).toHaveBeenCalledWith({
      email: 'new@test.com',
      password: 'Pass123',
    });
    expect(authService.saveToken).toHaveBeenCalledWith('new-token');
    expect(router.navigate).toHaveBeenCalledWith(['/profile']);
  });

  it('shows registration error', () => {
    authService.register.and.returnValue(throwError(() => ({ error: 'Email already exists' })));
    component.registerForm.setValue({
      name: 'New User',
      email: 'new@test.com',
      password: 'Pass123',
    });

    component.onSubmit();

    expect(component.errorMessage).toBe('Email already exists');
  });

  it('redirects to login when auto-login fails', () => {
    authService.register.and.returnValue(of('User registered'));
    authService.login.and.returnValue(throwError(() => ({ status: 401 })));
    component.registerForm.setValue({
      name: 'New User',
      email: 'new@test.com',
      password: 'Pass123',
    });

    component.onSubmit();

    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});

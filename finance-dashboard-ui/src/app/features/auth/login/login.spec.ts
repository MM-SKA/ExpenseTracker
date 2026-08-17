import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';

import {
  Router
} from '@angular/router';

import {
  of,
  throwError
} from 'rxjs';

import {
  vi
} from 'vitest';

import {
  LoginComponent
} from './login.component';

import {
  AuthService
} from '../../../core/services/auth.service';

import {
  CategoryService
} from '../../../core/services/category.service';

import {
  TEST_PROVIDERS
} from '../../../../testing/test-providers';

describe('LoginComponent', () => {

  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  let authServiceMock: {
    login: ReturnType<typeof vi.fn>;
  };

  let categoryServiceMock: {
    getCategories: ReturnType<typeof vi.fn>;
  };

  let router: Router;
  let navigateSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(async () => {

    authServiceMock = {
      login: vi.fn()
    };

    categoryServiceMock = {
      getCategories: vi.fn()
    };

    await TestBed
      .configureTestingModule({
        imports: [
          LoginComponent
        ],
        providers: [
          ...TEST_PROVIDERS,
          {
            provide: AuthService,
            useValue: authServiceMock
          },

          {
            provide: CategoryService,
            useValue: categoryServiceMock
          }
        ]
      })
      .compileComponents();

    fixture =
      TestBed.createComponent(LoginComponent);

    component =
      fixture.componentInstance;

    router =
      TestBed.inject(Router);

    navigateSpy =
      vi.spyOn(
        router,
        'navigate'
      ).mockResolvedValue(true);

    localStorage.clear();

    fixture.detectChanges();

  });

  afterEach(() => {
    vi.restoreAllMocks();
    localStorage.clear();
  });

  it('should create', () => {

    expect(component)
      .toBeTruthy();

  });

  it('should login successfully with valid credentials', () => {

    const response = {
      success: true,
      message: 'Login successful',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    authServiceMock.login
      .mockReturnValue(
        of(response)
      );

    categoryServiceMock.getCategories
      .mockReturnValue(
        of([])
      );

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.login();

    expect(authServiceMock.login)
      .toHaveBeenCalledWith({
        email: 'test@test.com',
        password: 'Test@123'
      });

    expect(localStorage.getItem('user'))
      .toBe(
        JSON.stringify(response.data)
      );

    expect(categoryServiceMock.getCategories)
      .toHaveBeenCalledWith(true);

    expect(navigateSpy)
      .toHaveBeenCalledWith([
        '/expenses'
      ]);

  });

  it('should still navigate to expenses if category preload fails after successful login', () => {

    const response = {
      success: true,
      message: 'Login successful',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    authServiceMock.login
      .mockReturnValue(
        of(response)
      );

    categoryServiceMock.getCategories
      .mockReturnValue(
        throwError(() => new Error('Category preload failed'))
      );

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.login();

    expect(localStorage.getItem('user'))
      .toBe(
        JSON.stringify(response.data)
      );

    expect(navigateSpy)
      .toHaveBeenCalledWith([
        '/expenses'
      ]);

  });

  it('should not navigate when password is incorrect', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.login
      .mockReturnValue(
        throwError(() => ({
          status: 401,
          error: {
            message: 'Invalid password'
          }
        }))
      );

    component.email =
      'test@test.com';

    component.password =
      'WrongPassword';

    component.login();

    expect(authServiceMock.login)
      .toHaveBeenCalledWith({
        email: 'test@test.com',
        password: 'WrongPassword'
      });

    expect(component.errorMessage)
      .toBe('Invalid password');

    expect(localStorage.getItem('user'))
      .toBeNull();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should not navigate when email is not found', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.login
      .mockReturnValue(
        throwError(() => ({
          status: 401,
          error: {
            message: 'Invalid email'
          }
        }))
      );

    component.email =
      '123@123.com';

    component.password =
      'Test@123';

    component.login();

    expect(authServiceMock.login)
      .toHaveBeenCalledWith({
        email: '123@123.com',
        password: 'Test@123'
      });

    expect(component.errorMessage)
      .toBe('Invalid email');

    expect(localStorage.getItem('user'))
      .toBeNull();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should not call login API when email format is invalid', () => {

    component.email =
      'invalid-email';

    component.password =
      'Test@123';

    component.login();

    expect(authServiceMock.login)
      .not
      .toHaveBeenCalled();

    expect(categoryServiceMock.getCategories)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Invalid email format');

  });

  it('should not call login API when password is empty', () => {

    component.email =
      'test@test.com';

    component.password =
      '';

    component.login();

    expect(authServiceMock.login)
      .not
      .toHaveBeenCalled();

    expect(categoryServiceMock.getCategories)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Password is required');

  });

  it('should not call login api when email is empty',()=>{
    component.email="";
    component.password="doesntMatter";
    component.login();
    expect(authServiceMock.login).not.toHaveBeenCalled();
    expect(navigateSpy).not.toHaveBeenCalled();
    expect(component.errorMessage).toBe('Email is required');
  });

});

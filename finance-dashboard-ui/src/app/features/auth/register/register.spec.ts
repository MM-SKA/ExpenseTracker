import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { Register } from './register';
import { AuthService } from '../../../core/services/auth.service';
import { TEST_PROVIDERS } from '../../../../testing/test-providers';

describe('Register', () => {

  let component: Register;
  let fixture: ComponentFixture<Register>;

  let authServiceMock: {
    register: ReturnType<typeof vi.fn>;
  };

  let router: Router;
  let navigateSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(async () => {

    authServiceMock = {
      register: vi.fn()
    };

    await TestBed
      .configureTestingModule({
        imports: [
          Register
        ],
        providers: [
          ...TEST_PROVIDERS,

          {
            provide: AuthService,
            useValue: authServiceMock
          }
        ]
      })
      .compileComponents();

    fixture =
      TestBed.createComponent(Register);

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

  it('should register successfully and navigate to login page', () => {

    const response = {
      success: true,
      message: 'User registered successfully',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    authServiceMock.register
      .mockReturnValue(
        of(response)
      );

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(authServiceMock.register)
      .toHaveBeenCalledWith({
        fullName: 'Test User',
        email: 'test@test.com',
        password: 'Test@123',
        phoneNumber: '9999999999'
      });

    expect(navigateSpy)
      .toHaveBeenCalledWith([
        '/'
      ]);

  });

  it('should trim full name, email and phone number before register API call', () => {

    const response = {
      success: true,
      message: 'User registered successfully',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    authServiceMock.register
      .mockReturnValue(
        of(response)
      );

    component.fullName =
      '  Test User  ';

    component.email =
      '  TEST@TEST.COM  ';

    component.password =
      'Test@123';

    component.phoneNumber =
      ' 9999999999 ';

    component.register();

    expect(authServiceMock.register)
      .toHaveBeenCalledWith({
        fullName: 'Test User',
        email: 'test@test.com',
        password: 'Test@123',
        phoneNumber: '9999999999'
      });

  });

  it('should not call register API when full name is empty', () => {

    component.fullName =
      '';

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Full name is required');

  });

  it('should not call register API when email format is invalid', () => {

    component.fullName =
      'Test User';

    component.email =
      'invalid-email';

    component.password =
      'Test@123';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Invalid email format');

  });

  it('should not call register API when password is empty', () => {

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      '';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Password is required');

  });

  it('should not call register API when phone number is invalid', () => {

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.phoneNumber =
      '12345';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Phone number must contain exactly 10 digits');

  });

  it('should not navigate when email already exists', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.register
      .mockReturnValue(
        throwError(() => ({
          status: 400,
          error: {
            success: false,
            message: 'Email already exists',
            errorCode: 'AUTH008'
          }
        }))
      );

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(authServiceMock.register)
      .toHaveBeenCalledWith({
        fullName: 'Test User',
        email: 'test@test.com',
        password: 'Test@123',
        phoneNumber: '9999999999'
      });

    expect(component.errorMessage)
      .toBe('Email already exists');

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should not navigate when phone number already exists', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.register
      .mockReturnValue(
        throwError(() => ({
          status: 400,
          error: {
            success: false,
            message: 'Phone number already exists',
            errorCode: 'AUTH008'
          }
        }))
      );

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(component.errorMessage)
      .toBe('Phone number already exists');

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should not navigate when backend rejects weak password', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.register
      .mockReturnValue(
        throwError(() => ({
          status: 400,
          error: {
            success: false,
            message: 'Password must contain uppercase, lowercase, number and symbol'
          }
        }))
      );

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      'weak';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(component.errorMessage)
      .toBe('Password must contain uppercase, lowercase, number and symbol');

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should not store token in localStorage after registration', () => {

    const response = {
      success: true,
      message: 'User registered successfully',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    authServiceMock.register
      .mockReturnValue(
        of(response)
      );

    component.fullName =
      'Test User';

    component.email =
      'test@test.com';

    component.password =
      'Test@123';

    component.phoneNumber =
      '9999999999';

    component.register();

    expect(localStorage.getItem('token'))
      .toBeNull();

    expect(localStorage.getItem('user'))
      .toBeNull();

  });

  it('should not call register API when all fields are empty', () => {

    component.fullName = '';
    component.email = '';
    component.password = '';
    component.phoneNumber = '';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Full name is required');

  });

  it('should not call register API when full name contains only spaces', () => {

    component.fullName = '     ';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '9999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Full name is required');

  });

  it('should lowercase and trim email before register API call', () => {

    const response = {
      success: true,
      message: 'User registered successfully',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    authServiceMock.register
      .mockReturnValue(
        of(response)
      );

    component.fullName = 'Test User';
    component.email = '  TEST@TEST.COM  ';
    component.password = 'Test@123';
    component.phoneNumber = '9999999999';

    component.register();

    expect(authServiceMock.register)
      .toHaveBeenCalledWith({
        fullName: 'Test User',
        email: 'test@test.com',
        password: 'Test@123',
        phoneNumber: '9999999999'
      });

  });

  it('should not call register API when phone number contains letters', () => {

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '99999abcde';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Phone number must contain exactly 10 digits');

  });

  it('should not call register API when phone number has more than 10 digits', () => {

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '99999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Phone number must contain exactly 10 digits');

  });

  it('should not call register API when phone number has less than 10 digits', () => {

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Phone number must contain exactly 10 digits');

  });

  it('should not call register API when password contains only spaces', () => {

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = '     ';
    component.phoneNumber = '9999999999';

    component.register();

    expect(authServiceMock.register)
      .not
      .toHaveBeenCalled();

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Password is required');

  });

  it('should not navigate when backend returns server error', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.register
      .mockReturnValue(
        throwError(() => ({
          status: 500,
          error: {
            success: false,
            message: 'Something went wrong'
          }
        }))
      );

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '9999999999';

    component.register();

    expect(authServiceMock.register)
      .toHaveBeenCalled();

    expect(component.errorMessage)
      .toBe('Something went wrong');

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(localStorage.getItem('token'))
      .toBeNull();

    expect(localStorage.getItem('user'))
      .toBeNull();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should show generic error when backend error message is missing', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    authServiceMock.register
      .mockReturnValue(
        throwError(() => ({
          status: 500,
          error: {}
        }))
      );

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '9999999999';

    component.register();

    expect(component.errorMessage)
      .toBe('Unable to register');

    expect(navigateSpy)
      .not
      .toHaveBeenCalled();

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

  });

  it('should clear previous error message before a new successful registration attempt', () => {

    const response = {
      success: true,
      message: 'User registered successfully',
      errorCode: null,
      data: {
        id: '1',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9999999999'
      }
    };

    component.errorMessage = 'Previous error';

    authServiceMock.register
      .mockReturnValue(
        of(response)
      );

    component.fullName = 'Test User';
    component.email = 'test@test.com';
    component.password = 'Test@123';
    component.phoneNumber = '9999999999';

    component.register();

    expect(component.errorMessage)
      .toBe('');

    expect(navigateSpy)
      .toHaveBeenCalledWith([
        '/'
      ]);

  });

});

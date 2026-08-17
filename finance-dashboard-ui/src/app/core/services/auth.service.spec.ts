import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { provideHttpClient } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { environment } from "../../../environment/environment";
import { AuthService } from "./auth.service";
import { LoginRequest } from "../../shared/models/auth/login-request";
import { LoginResponse } from "../../shared/models/auth/login-response";
import { ApiResponse } from "../../shared/models/api-response";
import { RegisterRequest } from "../../shared/models/auth/register-request";

describe("AuthService", () => {

  let service: AuthService;
  let httpMock: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => {
    httpMock.verify();
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should call login endpoint with credentials enabled", () => {
    const requestBody = {
      email: 'test@test.com',
      password: 'Test@123'
    };
    const responseBody: ApiResponse<LoginResponse> = {
      success: true,
      message: 'Login successful',
      errorCode: null,
      data: {
        user: {
          id: 'user-123',
          fullName: 'Test User',
          email: 'test@test.com',
          phoneNumber: '9876543210'
        },
        token: 'test-jwt-token'
      }
    };
    service.login(requestBody).subscribe(response => {
      expect(response).toEqual(responseBody);
      expect(response.success).toBe(true);
      expect(response.message).toBe('Login successful');
      expect(response.errorCode).toBeNull();
      expect(response.data.token).toBe('test-jwt-token');

      expect(response.data.user).toEqual({
        id: 'user-123',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9876543210'
      });
    });
    const request = httpMock.expectOne(
      `${environment.apiUrl}/auth/login`
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);
    expect(request.request.withCredentials).toBe(true);

    // Complete mocked HTTP request
    request.flush(responseBody);
  });

  it('should propagate login error when server returns 401', () => {
    const requestBody: LoginRequest = {
      email: 'test@test.com',
      password: 'Wrong@123'
    };

    service.login(requestBody).subscribe({
      next: () => {
        throw new Error('Expected login request to fail');
      },
      error: error => {
        expect(error.status).toBe(401);
      }
    });

    const request = httpMock.expectOne(
      `${environment.apiUrl}/auth/login`
    );

    expect(request.request.method).toBe('POST');

    request.flush(
      {
        success: false,
        message: 'Invalid credentials',
        errorCode: 'INVALID_CREDENTIALS',
        data: null
      },
      {
        status: 401,
        statusText: 'Unauthorized'
      }
    );
  });

  it('should call the register endpoint', () => {

    // Arrange
    const requestBody: RegisterRequest = {
      fullName: 'Test User',
      email: 'test@test.com',
      password: 'Test@123',
      phoneNumber: '9876543210'
    };

    const responseBody: ApiResponse<LoginResponse> = {
      success: true,
      message: 'Registration successful',
      errorCode: null,
      data: {
        user: {
          id: 'user-123',
          fullName: 'Test User',
          email: 'test@test.com',
          phoneNumber: '9876543210'
        },
        token: 'test-jwt-token'
      }
    };

    // Act
    service.register(requestBody).subscribe(response => {

      // Assert response
      expect(response).toEqual(responseBody);
      expect(response.success).toBe(true);
      expect(response.message).toBe('Registration successful');
      expect(response.errorCode).toBeNull();
      expect(response.data.token).toBe('test-jwt-token');

      expect(response.data.user).toEqual({
        id: 'user-123',
        fullName: 'Test User',
        email: 'test@test.com',
        phoneNumber: '9876543210'
      });
    });

    // Assert HTTP request
    const request = httpMock.expectOne(
      `${environment.apiUrl}/auth/register`
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);

    // Complete mocked request
    request.flush(responseBody);
  });

  it('should propagate register error when server returns 400', () => {

    const requestBody: RegisterRequest = {
      fullName: 'Test User',
      email: 'existing@test.com',
      password: 'Test@123',
      phoneNumber: '9876543210'
    };

    service.register(requestBody).subscribe({
      next: () => {
        throw new Error('Expected register request to fail');
      },
      error: error => {
        expect(error.status).toBe(400);
      }
    });

    const request = httpMock.expectOne(
      `${environment.apiUrl}/auth/register`
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);

    request.flush(
      {
        success: false,
        message: 'Email already exists',
        errorCode: 'EMAIL_ALREADY_EXISTS',
        data: null
      },
      {
        status: 400,
        statusText: 'Bad Request'
      }
    );
  });

  it('should call current user endpoint', () => {

    const responseBody: ApiResponse<LoginResponse> = {
      success: true,
      message: 'User found',
      errorCode: null,
      data: {
        user: {
          id: 'user-123',
          fullName: 'Test User',
          email: 'test@test.com',
          phoneNumber: '9876543210'
        },
        token: 'token'
      }
    };

    service
      .getCurrentUser()
      .subscribe(response => {

        expect(response)
          .toEqual(responseBody);

        expect(response.success)
          .toBe(true);

      });

    const request =
      httpMock.expectOne(
        `${environment.apiUrl}/auth/me`
      );

    expect(request.request.method)
      .toBe('GET');

    request.flush(responseBody);

  });
})

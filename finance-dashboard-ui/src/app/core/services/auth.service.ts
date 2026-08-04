import { inject ,Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environment/environment';

import { LoginRequest } from '../../shared/models/auth/login-request';
import { RegisterRequest } from '../../shared/models/auth/register-request';
import { LoginResponse } from '../../shared/models/auth/login-response';
import { ApiResponse } from '../../shared/models/api-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  readonly http = inject(HttpClient);

  login(
    request: LoginRequest
  ): Observable<ApiResponse<LoginResponse>>{
    return this.http.post<ApiResponse<LoginResponse>>(
      `${environment.apiUrl}/auth/login`,
      request);
  }

  register(
    request: RegisterRequest
  ): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${environment.apiUrl}/auth/register`,
      request);
  }
}

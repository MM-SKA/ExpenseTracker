import { inject ,Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { Observable } from "rxjs";

import { environment } from "../../../environment/environment";

import { LoginRequest } from '../../shared/models/auth/login-request';
import { LoginResponse } from '../../shared/models/auth/login-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  login(
    request: LoginRequest
  ): Observable<LoginResponse>{
    return this.http.post<LoginResponse>(
      `${environment.apiUrl}/auth/login`,
      request);
  }
}

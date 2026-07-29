import { AuthUser } from './auth-user';

export interface LoginResponse{
  user : AuthUser;
  token : string;
}

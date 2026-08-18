import { Observable } from 'rxjs';
import { ApiResponse } from '../api-response';

export interface ExpenseDto {
  id: string;
  description: string;
  amount: number;
  date: string;
  categoryId: string;
  categoryName: string;
  location?: string;
}


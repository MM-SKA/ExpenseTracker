export interface UpdateExpenseRequest {

  notes: string;

  amount: number;

  categoryId: string;

  date: string;

  location?: string;

}

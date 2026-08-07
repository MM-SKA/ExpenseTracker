export interface FilterExpense {

  categoryId?: string | null;

  startDate?: string | null;

  endDate?: string | null;

  minAmount?: number | null;

  maxAmount?: number | null;

  month?: number | null;

  year?: number | null;

  notes?: string | null;

  location?: string | null;

  includeAnalytics: boolean;

}

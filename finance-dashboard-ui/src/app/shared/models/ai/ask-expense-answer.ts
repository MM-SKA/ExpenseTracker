import { FilterExpense } from './filter-expense';

export interface AskExpenseAnswer {

  answer: string;

  filtersUsed: FilterExpense;

  analytics: any;

  expenses: any;

}

import { Locator, Page } from "@playwright/test";

export interface EditExpenseDetails {
  categoryId?: string;
  date: string;
  notes: string;
  amount: string;
  location: string;
}

export class EditExpensePage {
  readonly categoryDropdown: Locator;
  readonly dateInput: Locator;
  readonly notesInput: Locator;
  readonly amountInput: Locator;
  readonly locationInput: Locator;
  readonly updateButton: Locator;

  constructor(private readonly page: Page) {
    this.categoryDropdown = page.locator("#category-dropdown");

    this.dateInput = page.locator("#date-input");

    this.notesInput = page.locator("#notes-input");

    this.amountInput = page.locator("#amount-input");

    this.locationInput = page.locator("#location-input");

    this.updateButton = page.locator('button[type="submit"]');
  }

  async open(expenseId: string): Promise<void> {
    await this.page.goto(`/expenses/edit/${expenseId}`);

    await this.notesInput.waitFor({
      state: "visible",
    });
  }

  async waitForExpenseToLoad(): Promise<void> {
    await this.page.waitForFunction(() => {
      const notes = document.querySelector(
        "#notes-input",
      ) as HTMLInputElement | null;

      const amount = document.querySelector(
        "#amount-input",
      ) as HTMLInputElement | null;

      const date = document.querySelector(
        "#date-input",
      ) as HTMLInputElement | null;

      return Boolean(notes?.value && amount?.value && date?.value);
    });
  }

  async selectCategory(categoryId: string): Promise<void> {
    await this.categoryDropdown.selectOption(categoryId);
  }

  async fillExpense(expense: EditExpenseDetails): Promise<void> {
    if (expense.categoryId) {
      await this.selectCategory(expense.categoryId);
    }

    await this.dateInput.fill(expense.date);

    await this.notesInput.fill(expense.notes);

    await this.amountInput.fill(expense.amount);

    await this.locationInput.fill(expense.location);
  }

  async submit(): Promise<void> {
    await this.updateButton.click();
  }

  async updateExpense(expense: EditExpenseDetails): Promise<void> {
    await this.fillExpense(expense);

    await this.submit();
  }
}

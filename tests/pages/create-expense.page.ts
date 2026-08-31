import { Locator, Page } from "@playwright/test";

export interface CreateExpenseDetails {
  categoryId?: string;
  date: string;
  description: string;
  amount: string;
  location: string;
}

export class CreateExpensePage {
  readonly categorySelect: Locator;
  readonly dateInput: Locator;
  readonly descriptionInput: Locator;
  readonly amountInput: Locator;
  readonly locationInput: Locator;
  readonly submitButton: Locator;

  constructor(private readonly page: Page) {
    this.categorySelect = page.locator("#category-list");

    this.dateInput = page.locator("#date-input");

    this.descriptionInput = page.locator("#description-input");

    this.amountInput = page.locator("#amount-input");

    this.locationInput = page.locator("#location-input");

    this.submitButton = page.locator('button[type="submit"]');
  }

  async open(): Promise<void> {
    await this.page.goto("/expenses/create");

    await this.descriptionInput.waitFor({
      state: "visible",
    });
  }

  async waitForCategories(): Promise<void> {
    await this.categorySelect.waitFor({
      state: "visible",
    });

    await this.page.waitForFunction(() => {
      const categorySelect = document.querySelector(
        "#category-list",
      ) as HTMLSelectElement | null;

      if (!categorySelect) {
        return false;
      }

      const selectableOptions = Array.from(categorySelect.options).filter(
        (option) => Boolean(option.value) && !option.disabled,
      );

      return !categorySelect.disabled && selectableOptions.length > 0;
    });
  }

  async getSelectedCategoryId(): Promise<string> {
    await this.waitForCategories();

    return this.categorySelect.inputValue();
  }

  async selectCategory(categoryId: string): Promise<void> {
    await this.waitForCategories();

    await this.categorySelect.selectOption(categoryId);
  }

  async fillForm(expense: CreateExpenseDetails): Promise<void> {
    await this.waitForCategories();

    if (expense.categoryId) {
      await this.selectCategory(expense.categoryId);
    }

    await this.dateInput.fill(expense.date);

    await this.descriptionInput.fill(expense.description);

    await this.amountInput.fill(expense.amount);

    await this.locationInput.fill(expense.location);
  }

  async submit(): Promise<void> {
    await this.submitButton.click();
  }

  async createExpense(expense: CreateExpenseDetails): Promise<void> {
    await this.fillForm(expense);

    await this.submit();
  }
}

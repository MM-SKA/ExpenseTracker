import { Locator, Page } from "@playwright/test";

export interface AnalyticsFilterDetails {
  categoryId?: string;
  startDate?: string;
  endDate?: string;
  minAmount?: string;
  maxAmount?: string;
  notes?: string;
}

export class AnalyticsPage {
  readonly categorySelect: Locator;
  readonly startDateInput: Locator;
  readonly endDateInput: Locator;
  readonly minAmountInput: Locator;
  readonly maxAmountInput: Locator;
  readonly notesInput: Locator;
  readonly searchButton: Locator;
  readonly resetButton: Locator;
  readonly loadingIndicator: Locator;
  readonly kpiCards: Locator;
  readonly categoryCards: Locator;
  readonly emptyState: Locator;

  constructor(private readonly page: Page) {
    this.categorySelect = page.locator("#category-list");

    this.startDateInput = page.locator("#startDate-input");

    this.endDateInput = page.locator("#endDate-input");

    this.minAmountInput = page.locator("#minAmount-input");

    this.maxAmountInput = page.locator("#maxAmount-input");

    this.notesInput = page.locator("#notes-input");

    this.searchButton = page.locator(".an-btn-search");

    this.resetButton = page.locator(".an-btn-reset");

    this.loadingIndicator = page.locator(".an-loading");

    this.kpiCards = page.locator(".an-kpi-card");

    this.categoryCards = page.locator(".an-cat-card");

    this.emptyState = page.locator(".an-empty");
  }

  async open(): Promise<void> {
    await this.page.goto("/analytics");

    await this.categorySelect.waitFor({
      state: "visible",
    });
  }

  async waitForInitialRequest(): Promise<void> {
    await this.page.waitForResponse(
      (response) =>
        response.url().includes("/expenses/filter-paged") &&
        response.request().method() === "POST",
    );
  }

  async selectFirstCategory(): Promise<string> {
    await this.page.waitForFunction(() => {
      const select = document.querySelector(
        "#category-list",
      ) as HTMLSelectElement | null;

      if (!select) {
        return false;
      }

      return Array.from(select.options).some((option) => Boolean(option.value));
    });

    const categoryOption = this.categorySelect
      .locator('option:not([value=""])')
      .first();

    const categoryId = await categoryOption.getAttribute("value");

    if (!categoryId) {
      throw new Error("No category is available");
    }

    await this.categorySelect.selectOption(categoryId);

    return categoryId;
  }

  async fillFilters(filters: AnalyticsFilterDetails): Promise<void> {
    if (filters.categoryId !== undefined) {
      await this.categorySelect.selectOption(filters.categoryId);
    }

    if (filters.startDate !== undefined) {
      await this.startDateInput.fill(filters.startDate);
    }

    if (filters.endDate !== undefined) {
      await this.endDateInput.fill(filters.endDate);
    }

    if (filters.minAmount !== undefined) {
      await this.minAmountInput.fill(filters.minAmount);
    }

    if (filters.maxAmount !== undefined) {
      await this.maxAmountInput.fill(filters.maxAmount);
    }

    if (filters.notes !== undefined) {
      await this.notesInput.fill(filters.notes);
    }
  }

  async search(): Promise<void> {
    await this.searchButton.click();
  }

  async reset(): Promise<void> {
    await this.resetButton.click();
  }

  async applyFilters(filters: AnalyticsFilterDetails): Promise<void> {
    await this.fillFilters(filters);
    await this.search();
  }
}

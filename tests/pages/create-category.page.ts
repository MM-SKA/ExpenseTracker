import {
  Locator,
  Page
} from '@playwright/test';

export interface CategoryDetails {
  name: string;
}

export class CreateCategoryPage {

  readonly categoryInput: Locator;

  readonly submitButton: Locator;

  readonly loginPageLink: Locator;

  constructor(
    private readonly page: Page
  ) {

    this.categoryInput =
      page.locator(
        '#category-input'
      );

    this.submitButton =
      page.locator(
        '.cf-submit'
      );

  }

  async open(): Promise<void> {

    await this.page.goto(
      '/categories/create'
    );

    await this.categoryInput.waitFor({
      state: 'visible'
    });

  }

  async createCategory(
    category: CategoryDetails
  ): Promise<void> {

    await this.categoryInput.fill(
      category.name
    );

    await this.submitButton.click();

  }

}

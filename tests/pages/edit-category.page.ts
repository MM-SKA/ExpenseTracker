import {
  Locator,
  Page
} from '@playwright/test';

export class EditCategoryPage {

  readonly categoryInput: Locator;

  readonly updateButton: Locator;

  constructor(
    private readonly page: Page
  ) {

    this.categoryInput =
      page.locator(
        '#category-input'
      );

    this.updateButton =
      page.locator(
        '.cf-submit'
      );

  }

  async open(
    categoryId: string
  ): Promise<void> {

    await this.page.goto(
      `/categories/edit/${categoryId}`
    );

    await this.categoryInput.waitFor({
      state: 'visible'
    });

  }

  async updateCategory(
    name: string
  ): Promise<void> {

    await this.categoryInput.clear();

    await this.categoryInput.fill(
      name
    );

    await this.updateButton.click();

  }

}

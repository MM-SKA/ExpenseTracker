import { Locator, Page } from '@playwright/test';

export interface RegisterUser {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
}

export class RegisterPage {

  readonly fullNameInput: Locator;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly phoneNumberInput: Locator;
  readonly submitButton: Locator;
  readonly loginLink: Locator;

  constructor(
    private readonly page: Page
  ) {

    this.fullNameInput =
      page.locator('#fullName-input');

    this.emailInput =
      page.locator('#email-input');

    this.passwordInput =
      page.locator('#password-input');

    this.phoneNumberInput =
      page.locator('#phoneNumber-input');

    this.submitButton =
      page.locator('button[type="submit"]');

    this.loginLink =
      page.locator('a');
  }

  async open(): Promise<void> {

    await this.page.goto('/register');

    await this.fullNameInput.waitFor({
      state: 'visible'
    });

  }

  async register(
    user: RegisterUser
  ): Promise<void> {

    await this.fullNameInput.fill(
      user.fullName
    );

    await this.emailInput.fill(
      user.email
    );

    await this.passwordInput.fill(
      user.password
    );

    await this.phoneNumberInput.fill(
      user.phoneNumber
    );

    await this.submitButton.click();

  }

}

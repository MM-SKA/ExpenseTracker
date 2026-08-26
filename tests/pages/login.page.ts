import { Locator, Page } from "@playwright/test";

export class LoginPage {
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly loginButton: Locator;

  constructor(private readonly page: Page) {
    this.emailInput = page.locator("#email-input");
    this.passwordInput = page.locator("#password-input");
    this.loginButton = page.locator('button[type="submit"]');
  }

  async open(): Promise<void> {
    await this.page.goto("/");
    console.log("URL:", await this.page.url());
  }

  async login(email: string, password: string): Promise<void> {
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.loginButton.click();
  }
}

import { expect, test } from "@playwright/test";
import { LoginPage } from "../pages/login.page";

test.describe("Login flow", () => {
  test("should login with valid credentials", async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.open();
    await loginPage.login("test@test.com", "Test@123");
    await expect(page).toHaveURL(/\/expenses/);
  });

  test("should remain on login page for invalid credentials", async ({page}) => {
    const loginPage = new LoginPage(page);
    await loginPage.open();
    await loginPage.login("test@test.com", "Wrong@123");
    await expect(page).not.toHaveURL(/\/expenses/);
  });
});

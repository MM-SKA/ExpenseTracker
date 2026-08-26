import { expect, test } from "@playwright/test";
import { RegisterPage } from "../pages/register.page";

test.describe("Register Flow", () => {
  test("should load register page", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    await expect(registerPage.fullNameInput).toBeVisible();

    await expect(registerPage.emailInput).toBeVisible();

    await expect(registerPage.passwordInput).toBeVisible();

    await expect(registerPage.phoneNumberInput).toBeVisible();
  });
  test("should register successfully", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    const unique = Date.now();

    await registerPage.register({
      fullName: "Playwright User",

      email: `playwright${unique}@test.com`,

      password: "Test@123",

      phoneNumber: "9876543210",
    });

    await expect(page).toHaveURL(/|expenses/);
  });
  test("should reject invalid email", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    await registerPage.register({
      fullName: "Playwright User",

      email: "invalid-email",

      password: "Test@123",

      phoneNumber: "9876543210",
    });

    await expect(page).toHaveURL(/register/);
  });
  test("should reject empty full name", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    await registerPage.register({
      fullName: "",

      email: "user@test.com",

      password: "Test@123",

      phoneNumber: "9876543210",
    });

    await expect(page).toHaveURL(/register/);
  });
  test("should reject empty password", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    await registerPage.register({
      fullName: "Playwright User",

      email: "user@test.com",

      password: "",

      phoneNumber: "9876543210",
    });

    await expect(page).toHaveURL(/register/);
  });
  test("should reject empty phone number", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    await registerPage.register({
      fullName: "Playwright User",

      email: "user@test.com",

      password: "Test@123",

      phoneNumber: "",
    });

    await expect(page).toHaveURL(/register/);
  });
  test("should reject duplicate email", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    await registerPage.register({
      fullName: "Existing User",

      email: "test@test.com",

      password: "Test@123",

      phoneNumber: "9876543210",
    });

    await expect(page).toHaveURL(/register/);
  });

  test("should reject duplicate phone number", async ({ page }) => {
    const registerPage = new RegisterPage(page);

    await registerPage.open();

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/auth/register") &&
        response.request().method() === "POST",
    );

    await registerPage.register({
      fullName: "Existing User",

      email: `duplicate-${Date.now()}@test.com`,

      password: "Strong@Test123",
      phoneNumber: "1234567890",
    });

    const response = await responsePromise;

    expect(response.ok()).toBeFalsy();

    await expect(page).toHaveURL(/register/);
  });
});

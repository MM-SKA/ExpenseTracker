import { expect, test } from "@playwright/test";

import { LoginPage } from "../pages/login.page";

import { CreateCategoryPage } from "../pages/create-category.page";

test.describe("Create Category Flow", () => {
  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.open();

    await loginPage.login("test@test.com", "Test@123");

    await expect(page).toHaveURL(/expenses/);
  });
  test("should load create category page", async ({ page }) => {
    const createCategoryPage = new CreateCategoryPage(page);

    await createCategoryPage.open();

    await expect(createCategoryPage.categoryInput).toBeVisible();

    await expect(createCategoryPage.submitButton).toBeVisible();
  });
  test("should create category successfully", async ({ page }) => {
    const createCategoryPage = new CreateCategoryPage(page);

    await createCategoryPage.open();

    const categoryName = `Playwright-${Date.now()}`;

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/categories/create") &&
        response.request().method() === "POST",
    );

    await createCategoryPage.createCategory({
      name: categoryName,
    });

    const response = await responsePromise;

    expect(response.ok()).toBeTruthy();

    await expect(page).toHaveURL(/categories/);
  });

  test("should send correct category payload", async ({ page }) => {
    const createCategoryPage = new CreateCategoryPage(page);

    await createCategoryPage.open();

    const categoryName = `Payload-${Date.now()}`;

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes("/categories/create") &&
        request.method() === "POST",
    );

    await createCategoryPage.createCategory({
      name: categoryName,
    });

    const request = await requestPromise;

    expect(request.postDataJSON()).toEqual({
      name: categoryName,
    });
  });

  test("should reject empty category name", async ({ page }) => {
    const createCategoryPage = new CreateCategoryPage(page);

    await createCategoryPage.open();

    let requestCount = 0;

    page.on("request", (request) => {
      if (request.url().includes("/categories/create")) {
        requestCount++;
      }
    });

    await createCategoryPage.createCategory({
      name: "",
    });

    expect(requestCount).toBe(0);
  });

  test("should reject whitespace-only category", async ({ page }) => {
    const createCategoryPage = new CreateCategoryPage(page);

    await createCategoryPage.open();

    let requestCount = 0;

    page.on("request", (request) => {
      if (request.url().includes("/categories/create")) {
        requestCount++;
      }
    });

    await createCategoryPage.createCategory({
      name: "     ",
    });

    expect(requestCount).toBe(0);
  });
  
});

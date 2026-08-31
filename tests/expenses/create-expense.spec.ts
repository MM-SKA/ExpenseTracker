import { expect, Page, test } from "@playwright/test";

import { LoginPage } from "../pages/login.page";

import {
  CreateExpenseDetails,
  CreateExpensePage,
} from "../pages/create-expense.page";

function getTodayDate(): string {
  const today = new Date();

  const year = today.getFullYear();

  const month = String(today.getMonth() + 1).padStart(2, "0");

  const day = String(today.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
}

function getFutureDate(): string {
  const futureDate = new Date();

  futureDate.setDate(futureDate.getDate() + 2);

  const year = futureDate.getFullYear();

  const month = String(futureDate.getMonth() + 1).padStart(2, "0");

  const day = String(futureDate.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
}

async function countCreateRequests(page: Page): Promise<{
  getCount: () => number;
}> {
  let requestCount = 0;

  await page.route("**/expenses/create", async (route) => {
    requestCount++;

    await route.continue();
  });

  return {
    getCount: () => requestCount,
  };
}

test.describe("Create Expense Flow", () => {
  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.open();

    await loginPage.login("test@test.com", "Test@123");

    await expect(page).toHaveURL(/\/expenses/);
  });

  test("should load create expense page", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    await expect(createExpensePage.categorySelect).toBeVisible();

    await expect(createExpensePage.dateInput).toBeVisible();

    await expect(createExpensePage.descriptionInput).toBeVisible();

    await expect(createExpensePage.amountInput).toBeVisible();

    await expect(createExpensePage.locationInput).toBeVisible();

    await expect(createExpensePage.submitButton).toBeVisible();
  });

  test("should load categories into dropdown", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    await createExpensePage.waitForCategories();

    await expect(createExpensePage.categorySelect).toBeEnabled();

    const selectedCategoryId = await createExpensePage.getSelectedCategoryId();

    expect(selectedCategoryId).not.toBe("");
  });

  test("should create expense successfully", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const expense: CreateExpenseDetails = {
      date: getTodayDate(),

      description: `Playwright Lunch ${Date.now()}`,

      amount: "500.50",

      location: "Vadodara",
    };

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/expenses/create") &&
        response.request().method() === "POST",
    );

    await createExpensePage.createExpense(expense);

    const response = await responsePromise;

    expect(response.ok()).toBe(true);

    expect(response.status()).toBeGreaterThanOrEqual(200);

    expect(response.status()).toBeLessThan(300);

    await expect(page).toHaveURL(/\/expenses$/);
  });

  test("should send correct expense payload", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const categoryId = await createExpensePage.getSelectedCategoryId();

    const expense: CreateExpenseDetails = {
      categoryId,

      date: getTodayDate(),

      description: `Payload Test ${Date.now()}`,

      amount: "250.75",

      location: "Vadodara",
    };

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes("/expenses/create") &&
        request.method() === "POST",
    );

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/expenses/create") &&
        response.request().method() === "POST",
    );

    await createExpensePage.createExpense(expense);

    const request = await requestPromise;

    const requestBody: Record<string, unknown> = request.postDataJSON();

    expect(requestBody).toEqual({
      categoryId,
      amount: 250.75,
      notes: expense.description,
      date: expense.date,
      location: expense.location,
    });

    const response = await responsePromise;

    expect(response.ok()).toBe(true);
  });

  test("should support decimal amount", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes("/expenses/create") &&
        request.method() === "POST",
    );

    await createExpensePage.createExpense({
      date: getTodayDate(),

      description: `Decimal Test ${Date.now()}`,

      amount: "99.95",

      location: "Vadodara",
    });

    const request = await requestPromise;

    const requestBody: Record<string, unknown> = request.postDataJSON();

    expect(requestBody["amount"]).toBe(99.95);
  });

  test("should allow empty location", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes("/expenses/create") &&
        request.method() === "POST",
    );

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/expenses/create") &&
        response.request().method() === "POST",
    );

    await createExpensePage.createExpense({
      date: getTodayDate(),

      description: `No Location ${Date.now()}`,

      amount: "100",

      location: "",
    });

    const request = await requestPromise;

    const requestBody: Record<string, unknown> = request.postDataJSON();

    expect(requestBody["location"]).toBe("");

    const response = await responsePromise;

    expect(response.ok()).toBe(true);

    await expect(page).toHaveURL(/\/expenses$/);
  });

  test("should show success toast after creating expense", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/expenses/create") &&
        response.request().method() === "POST",
    );

    await createExpensePage.createExpense({
      date: getTodayDate(),

      description: `Toast Test ${Date.now()}`,

      amount: "600",

      location: "Vadodara",
    });

    const response = await responsePromise;

    expect(response.ok()).toBe(true);

    await expect(page.getByText("Expense created successfully.")).toBeVisible();

    await expect(page).toHaveURL(/\/expenses$/);
  });

  test("should reject empty date", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const requestTracker = await countCreateRequests(page);

    await createExpensePage.createExpense({
      date: "",
      description: "Lunch",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Expense date is required")).toBeVisible();

    expect(requestTracker.getCount()).toBe(0);

    await expect(page).toHaveURL(/\/expenses\/create/);
  });

  test("should reject future expense date", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const requestTracker = await countCreateRequests(page);

    await createExpensePage.createExpense({
      date: getFutureDate(),
      description: "Future Expense",
      amount: "500",
      location: "Vadodara",
    });

    await expect(
      page.getByText("Expense date cannot be in the future"),
    ).toBeVisible();

    expect(requestTracker.getCount()).toBe(0);

    await expect(page).toHaveURL(/\/expenses\/create/);
  });

  test("should reject empty description", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const requestTracker = await countCreateRequests(page);

    await createExpensePage.createExpense({
      date: getTodayDate(),
      description: "",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Description is required")).toBeVisible();

    expect(requestTracker.getCount()).toBe(0);
  });

  test("should reject whitespace-only description", async ({ page }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    const requestTracker = await countCreateRequests(page);

    await createExpensePage.createExpense({
      date: getTodayDate(),
      description: "     ",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Description is required")).toBeVisible();

    expect(requestTracker.getCount()).toBe(0);
  });

  const invalidAmounts = [
    {
      caseName: "empty amount",
      amount: "",
    },
    {
      caseName: "zero amount",
      amount: "0",
    },
    {
      caseName: "negative amount",
      amount: "-100",
    },
  ];

  for (const invalidCase of invalidAmounts) {
    test(`should reject ${invalidCase.caseName}`, async ({ page }) => {
      const createExpensePage = new CreateExpensePage(page);

      await createExpensePage.open();

      let requestCount = 0;

      page.on("request", (request) => {
        if (
          request.url().includes("/expenses/create") &&
          request.method() === "POST"
        ) {
          requestCount++;
        }
      });

      await createExpensePage.createExpense({
        date: getTodayDate(),
        description: "Lunch",
        amount: invalidCase.amount,
        location: "Vadodara",
      });

      await expect(
        page.getByText("Amount must be greater than zero"),
      ).toBeVisible();

      expect(requestCount).toBe(0);
    });
  }

  test("should show error toast when API returns an error", async ({
    page,
  }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    await page.route("**/expenses/create", async (route) => {
      await route.fulfill({
        status: 500,

        contentType: "application/json",

        body: JSON.stringify({
          success: false,
          message: "Internal server error",
          errorCode: "SERVER_ERROR",
          data: null,
        }),
      });
    });

    await createExpensePage.createExpense({
      date: getTodayDate(),
      description: "Failed Expense",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Unable to create expense.")).toBeVisible();

    await expect(page).toHaveURL(/\/expenses\/create/);
  });

  test("should remain on create page when network request fails", async ({
    page,
  }) => {
    const createExpensePage = new CreateExpensePage(page);

    await createExpensePage.open();

    await page.route("**/expenses/create", async (route) => {
      await route.abort("connectionfailed");
    });

    await createExpensePage.createExpense({
      date: getTodayDate(),
      description: "Network Failure Test",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Unable to create expense.")).toBeVisible();

    await expect(page).toHaveURL(/\/expenses\/create/);
  });
});

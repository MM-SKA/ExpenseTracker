import { expect, test } from "@playwright/test";

import { LoginPage } from "../pages/login.page";

import {
  EditExpenseDetails,
  EditExpensePage,
} from "../pages/edit-expense.page";

const EXPENSE_ID = "204d6ab8-8ae3-4341-bfd7-f8899fc4a720";

test.describe("Edit expense flow", () => {
  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.open();

    await loginPage.login("test@test.com", "Test@123");

    await expect(page).toHaveURL(/\/expenses/);
  });

  test("should render edit expense form", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await expect(editPage.categoryDropdown).toBeVisible();

    await expect(editPage.dateInput).toBeVisible();

    await expect(editPage.notesInput).toBeVisible();

    await expect(editPage.amountInput).toBeVisible();

    await expect(editPage.locationInput).toBeVisible();

    await expect(editPage.updateButton).toBeVisible();
  });

  test("should load existing expense values", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    await expect(editPage.notesInput).not.toHaveValue("");

    await expect(editPage.amountInput).not.toHaveValue("");

    await expect(editPage.dateInput).not.toHaveValue("");

    await expect(editPage.categoryDropdown).not.toHaveValue("");
  });

  test("should update expense successfully", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    const selectedCategoryId = await editPage.categoryDropdown.inputValue();

    const updatedExpense: EditExpenseDetails = {
      categoryId: selectedCategoryId,

      date: "2026-08-20",

      notes: `Updated expense ${Date.now()}`,

      amount: "650.75",

      location: "Vadodara",
    };

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes(`/expenses/update/${EXPENSE_ID}`) &&
        request.method() === "PUT",
    );

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes(`/expenses/update/${EXPENSE_ID}`) &&
        response.request().method() === "PUT",
    );

    await editPage.updateExpense(updatedExpense);

    const request = await requestPromise;

    const response = await responsePromise;

    expect(request.postDataJSON()).toEqual({
      notes: updatedExpense.notes,

      amount: 650.75,

      categoryId: selectedCategoryId,

      date: updatedExpense.date,

      location: updatedExpense.location,
    });

    expect(response.ok()).toBe(true);

    await expect(page).toHaveURL(/\/expenses$/);
  });

  test("should display success toast after update", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    const categoryId = await editPage.categoryDropdown.inputValue();

    await editPage.updateExpense({
      categoryId,
      date: "2026-08-20",
      notes: `Toast update ${Date.now()}`,
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Expense edited successfully.")).toBeVisible();

    await expect(page).toHaveURL(/\/expenses$/);
  });

  test("should persist updated expense after reopening edit page", async ({
    page,
  }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    const categoryId = await editPage.categoryDropdown.inputValue();

    const updatedNotes = `Persistent update ${Date.now()}`;

    await editPage.updateExpense({
      categoryId,
      date: "2026-08-20",
      notes: updatedNotes,
      amount: "700.50",
      location: "Ahmedabad",
    });

    await expect(page).toHaveURL(/\/expenses$/);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    await expect(editPage.notesInput).toHaveValue(updatedNotes);

    await expect(editPage.amountInput).toHaveValue("700.5");

    await expect(editPage.locationInput).toHaveValue("Ahmedabad");
  });

  test("should support decimal amount", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    const requestPromise = page.waitForRequest((request) =>
      request.url().includes(`/expenses/update/${EXPENSE_ID}`),
    );

    await editPage.updateExpense({
      date: "2026-08-20",
      notes: "Decimal update",
      amount: "99.95",
      location: "Vadodara",
    });

    const request = await requestPromise;

    const requestBody = request.postDataJSON();

    expect(requestBody.amount).toBe(99.95);
  });

  test("should allow empty location", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    const responsePromise = page.waitForResponse((response) =>
      response.url().includes(`/expenses/update/${EXPENSE_ID}`),
    );

    await editPage.updateExpense({
      date: "2026-08-20",
      notes: "No location update",
      amount: "125",
      location: "",
    });

    const response = await responsePromise;

    expect(response.ok()).toBe(true);

    await expect(page).toHaveURL(/\/expenses$/);
  });

  test("should reject empty date", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    let updateRequestCount = 0;

    page.on("request", (request) => {
      if (
        request.url().includes("/expenses/update/") &&
        request.method() === "PUT"
      ) {
        updateRequestCount++;
      }
    });

    await editPage.updateExpense({
      date: "",
      notes: "Lunch",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Expense date is required")).toBeVisible();

    expect(updateRequestCount).toBe(0);

    await expect(page).toHaveURL(/\/expenses\/edit/);
  });

  test("should reject a future date", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    const futureDate = new Date();

    futureDate.setDate(futureDate.getDate() + 2);

    const futureDateValue = futureDate.toISOString().slice(0, 10);

    let updateRequestCount = 0;

    page.on("request", (request) => {
      if (request.url().includes("/expenses/update/")) {
        updateRequestCount++;
      }
    });

    await editPage.updateExpense({
      date: futureDateValue,
      notes: "Future expense",
      amount: "500",
      location: "Vadodara",
    });

    await expect(
      page.getByText("Expense date cannot be in the future"),
    ).toBeVisible();

    expect(updateRequestCount).toBe(0);
  });

  test("should reject empty description", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    let updateRequestCount = 0;

    page.on("request", (request) => {
      if (request.url().includes("/expenses/update/")) {
        updateRequestCount++;
      }
    });

    await editPage.updateExpense({
      date: "2026-08-20",
      notes: "",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Description is required")).toBeVisible();

    expect(updateRequestCount).toBe(0);
  });

  test("should reject whitespace-only description", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    let updateRequestCount = 0;

    page.on("request", (request) => {
      if (request.url().includes("/expenses/update/")) {
        updateRequestCount++;
      }
    });

    await editPage.updateExpense({
      date: "2026-08-20",
      notes: "     ",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page.getByText("Description is required")).toBeVisible();

    expect(updateRequestCount).toBe(0);
  });

  test("should reject empty amount", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    let updateRequestCount = 0;

    page.on("request", (request) => {
      if (request.url().includes("/expenses/update/")) {
        updateRequestCount++;
      }
    });

    await editPage.updateExpense({
      date: "2026-08-20",
      notes: "Lunch",
      amount: "",
      location: "Vadodara",
    });

    await expect(page.getByText("Amount is required")).toBeVisible();

    expect(updateRequestCount).toBe(0);
  });

  test("should remain on edit page when update API fails", async ({ page }) => {
    const editPage = new EditExpensePage(page);

    await editPage.open(EXPENSE_ID);

    await editPage.waitForExpenseToLoad();

    await page.route(`**/expenses/update/${EXPENSE_ID}`, async (route) => {
      await route.fulfill({
        status: 500,

        contentType: "application/json",

        body: JSON.stringify({
          success: false,
          message: "Unable to update expense",
        }),
      });
    });

    await editPage.updateExpense({
      date: "2026-08-20",
      notes: "Failed update",
      amount: "500",
      location: "Vadodara",
    });

    await expect(page).toHaveURL(/\/expenses\/edit/);
  });
});

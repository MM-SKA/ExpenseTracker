import { expect, test } from "@playwright/test";

import { LoginPage } from "../pages/login.page";

test.describe("Delete Category Flow", () => {
  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.open();

    await loginPage.login("test@test.com", "Test@123");

    await expect(page).toHaveURL(/expenses/);
  });
  test("should load category list", async ({ page }) => {
    await page.goto("/categories");

    await expect(page.locator(".cl-card").first()).toBeVisible();
  });
  test("should not delete when user cancels confirmation", async ({ page }) => {
    await page.goto("/categories");

    page.once("dialog", async (dialog) => {
      expect(dialog.message()).toContain("Are you sure");

      await dialog.dismiss();
    });

    const countBefore = await page.locator(".cl-card").count();

    await page.locator(".delete-button").first().click();

    const countAfter = await page.locator(".cl-card").count();

    expect(countAfter).toBe(countBefore);
  });
  test("should delete category successfully", async ({ page }) => {
    // Create category first

    await page.goto("/categories");

    console.log(await page.locator(".delete-button").count());

    const categoryName = `DeleteTest-${Date.now()}`;

    await page.goto("/categories/create");

    await page.locator("#category-input").fill(categoryName);

    await page.locator(".cf-submit").click();

    await expect(page).toHaveURL(/categories/);

    // Find newly created category

    const card = page.locator(".cl-card").filter({
      hasText: categoryName,
    });

    await expect(card).toBeVisible();

    page.once("dialog", async (dialog) => {
      await dialog.accept();
    });

    const deletePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/categories/delete/") &&
        response.request().method() === "DELETE",
    );

    await card.locator(".delete-button").click();

    const response = await deletePromise;

    expect(response.ok()).toBeTruthy();
  });

  test("should call delete endpoint", async ({ page }) => {
    await page.goto("/categories");

    page.once("dialog", async (dialog) => {
      await dialog.accept();
    });

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes("/categories/delete/") &&
        request.method() === "DELETE",
    );

    await page.locator(".delete-button").last().click();

    const request = await requestPromise;

    expect(request.method()).toBe("DELETE");
  });
  test("should remove deleted category from list", async ({ page }) => {
    await page.goto("/categories");

    const cards = page.locator(".cl-card");

    const countBefore = await cards.count();

    page.once("dialog", async (dialog) => {
      await dialog.accept();
    });

    await page.locator(".delete-button").last().click();

    await expect(cards).toHaveCount(countBefore - 1);
  });
  test("should show error when delete fails", async ({ page }) => {
    await page.goto("/categories");

    await page.route("**/categories/delete/*", async (route) => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({
          message: "Server Error",
        }),
      });
    });

    page.once("dialog", async (dialog) => {
      await dialog.accept();
    });

    await page.locator(".delete-button").last().click();

    await expect(page.getByText(/Unable to delete category/i)).toBeVisible();
  });
});

import { expect, test } from "@playwright/test";

import { LoginPage } from "../pages/login.page";

import { EditCategoryPage } from "../pages/edit-category.page";

test.describe("Edit Category Flow", () => {
  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.open();

    await loginPage.login("test@test.com", "Test@123");

    await expect(page).toHaveURL(/expenses/);
  });
  test("should load edit category page", async ({ page }) => {
    const categoryId = "37de4e25-bc53-42f2-b710-132b0641445f";

    const editPage = new EditCategoryPage(page);

    await editPage.open(categoryId);

    await expect(editPage.categoryInput).toBeVisible();

    const value = await editPage.categoryInput.inputValue();

    expect(value.length).toBe(0);
  });
  test("should load category name from api", async ({ page }) => {
    const categoryId = "37de4e25-bc53-42f2-b710-132b0641445f";

    const editPage = new EditCategoryPage(page);

    await editPage.open(categoryId);

    await expect(editPage.categoryInput).toHaveValue("");
  });
  test("should update category successfully", async ({ page }) => {
    const categoryId = "37de4e25-bc53-42f2-b710-132b0641445f";

    const editPage = new EditCategoryPage(page);

    await editPage.open(categoryId);

    const updatedName = `Updated-${Date.now()}`;

    const requestPromise = page.waitForRequest(
      (request) =>
        request.url().includes(`/categories/update/${categoryId}`) &&
        request.method() === "PUT",
    );

    await editPage.updateCategory(updatedName);

    const request = await requestPromise;

    expect(request.postDataJSON()).toEqual({
      name: updatedName,
    });

    await expect(page).toHaveURL(/categories/);
  });
});

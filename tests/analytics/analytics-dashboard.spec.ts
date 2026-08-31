import { expect, Page, test } from "@playwright/test";

import { LoginPage } from "../pages/login.page";

import { AnalyticsPage } from "../pages/analytics.page";

interface AnalyticsRequestBody {
  categoryId?: string;
  startDate?: string;
  endDate?: string;
  minAmount?: number;
  maxAmount?: number;
  notes?: string;
  location?: string;
  includeAnalytics: boolean;
  pageNumber: number;
  pageSize: number;
  sortOrder: "recent" | "oldest";
}

const analyticsResponse = {
  success: true,
  message: "Analytics fetched successfully",
  errorCode: null,
  data: {
    items: [],
    pageNumber: 1,
    pageSize: 10,
    totalRecords: 3,
    totalPages: 1,
    analytics: {
      summary: {
        totalSpent: 1500,
        count: 3,
        averageAmount: 500,
        minAmount: 250,
        maxAmount: 750,
      },
      byCategory: [
        {
          categoryId: "category-food",
          categoryName: "Food",
          amount: 1000,
          transactionCount: 2,
          percentage: 66.67,
        },
        {
          categoryId: "category-travel",
          categoryName: "Travel",
          amount: 500,
          transactionCount: 1,
          percentage: 33.33,
        },
      ],
      dateDistribution: {
        startDate: "2026-08-01T00:00:00Z",
        endDate: "2026-08-20T00:00:00Z",
        daysWithExpenses: 3,
      },
    },
  },
};

async function mockAnalyticsResponse(
  page: Page,
  responseBody: unknown = analyticsResponse,
): Promise<void> {
  await page.route("**/expenses/filter-paged", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(responseBody),
    });
  });
}

async function trackAnalyticsRequests(page: Page): Promise<{
  getCount: () => number;
}> {
  let requestCount = 0;

  page.on("request", (request) => {
    if (
      request.url().includes("/expenses/filter-paged") &&
      request.method() === "POST"
    ) {
      requestCount++;
    }
  });

  return {
    getCount: () => requestCount,
  };
}

test.describe("Analytics Dashboard", () => {
  test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.open();

    await loginPage.login("test@test.com", "Test@123");

    await expect(page).toHaveURL(/\/expenses/);
  });

  test("should render analytics filters", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    await expect(analyticsPage.categorySelect).toBeVisible();

    await expect(analyticsPage.startDateInput).toBeVisible();

    await expect(analyticsPage.endDateInput).toBeVisible();

    await expect(analyticsPage.minAmountInput).toBeVisible();

    await expect(analyticsPage.maxAmountInput).toBeVisible();

    await expect(analyticsPage.notesInput).toBeVisible();

    await expect(analyticsPage.searchButton).toBeVisible();

    await expect(analyticsPage.resetButton).toBeVisible();
  });

  test("should load analytics on page initialization", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const responsePromise = page.waitForResponse(
      (response) =>
        response.url().includes("/expenses/filter-paged") &&
        response.request().method() === "POST",
    );

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const response = await responsePromise;

    expect(response.ok()).toBe(true);

    await expect(analyticsPage.kpiCards).toHaveCount(4);
  });

  test("should send default analytics request", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const requestPromise = page.waitForRequest((request) => {
      if (
        !request.url().includes("/expenses/filter-paged") ||
        request.method() !== "POST"
      ) {
        return false;
      }

      const body = request.postDataJSON() as AnalyticsRequestBody;

      return (
        body.pageNumber === 1 &&
        body.pageSize === 10 &&
        body.sortOrder === "recent" &&
        body.includeAnalytics === true
      );
    });

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const request = await requestPromise;

    const body = request.postDataJSON() as AnalyticsRequestBody;

    expect(body).toEqual({
      pageNumber: 1,
      pageSize: 10,
      sortOrder: "recent",
      includeAnalytics: true,
    });
  });

  test("should display KPI analytics values", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    await expect(analyticsPage.kpiCards).toHaveCount(4);

    await expect(page.locator(".an-kpi-card").nth(0)).toContainText("1,500");

    await expect(page.locator(".an-kpi-card").nth(1)).toContainText("3");

    await expect(page.locator(".an-kpi-card").nth(2)).toContainText("500");
  });

  test("should display category breakdown", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    await expect(analyticsPage.categoryCards).toHaveCount(2);

    await expect(analyticsPage.categoryCards.nth(0)).toContainText("Food");

    await expect(analyticsPage.categoryCards.nth(0)).toContainText("67%");

    await expect(analyticsPage.categoryCards.nth(1)).toContainText("Travel");

    await expect(analyticsPage.categoryCards.nth(1)).toContainText("33%");
  });

  test("should apply all analytics filters", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const categoryId = await analyticsPage.selectFirstCategory();

    const requestPromise = page.waitForRequest((request) => {
      if (
        !request.url().includes("/expenses/filter-paged") ||
        request.method() !== "POST"
      ) {
        return false;
      }

      const body = request.postDataJSON() as AnalyticsRequestBody;

      return (
        body.categoryId === categoryId &&
        body.startDate === "2026-08-01" &&
        body.endDate === "2026-08-20" &&
        body.minAmount === 100 &&
        body.maxAmount === 1000 &&
        body.notes === "Lunch"
      );
    });

    await analyticsPage.applyFilters({
      categoryId,
      startDate: "2026-08-01",
      endDate: "2026-08-20",
      minAmount: "100",
      maxAmount: "1000",
      notes: "Lunch",
    });

    const request = await requestPromise;

    const body = request.postDataJSON() as AnalyticsRequestBody;

    expect(body).toEqual({
      categoryId,
      startDate: "2026-08-01",
      endDate: "2026-08-20",
      minAmount: 100,
      maxAmount: 1000,
      notes: "Lunch",
      includeAnalytics: true,
      pageNumber: 1,
      pageSize: 10,
      sortOrder: "recent",
    });
  });

  test("should trim search notes before sending request", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const requestPromise = page.waitForRequest((request) => {
      if (!request.url().includes("/expenses/filter-paged")) {
        return false;
      }

      const body = request.postDataJSON() as AnalyticsRequestBody;

      return body.notes === "Coffee";
    });

    await analyticsPage.applyFilters({
      notes: "   Coffee   ",
    });

    const request = await requestPromise;

    const body = request.postDataJSON() as AnalyticsRequestBody;

    expect(body.notes).toBe("Coffee");
  });

  test("should reset all analytics filters", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const categoryId = await analyticsPage.selectFirstCategory();

    await analyticsPage.fillFilters({
      categoryId,
      startDate: "2026-08-01",
      endDate: "2026-08-20",
      minAmount: "100",
      maxAmount: "1000",
      notes: "Lunch",
    });

    const resetRequestPromise = page.waitForRequest((request) => {
      if (
        !request.url().includes("/expenses/filter-paged") ||
        request.method() !== "POST"
      ) {
        return false;
      }

      const body = request.postDataJSON() as AnalyticsRequestBody;

      return (
        body.pageNumber === 1 &&
        body.pageSize === 10 &&
        body.sortOrder === "recent" &&
        body.includeAnalytics === true &&
        !body.categoryId &&
        !body.startDate &&
        !body.endDate &&
        body.minAmount === undefined &&
        body.maxAmount === undefined &&
        !body.notes
      );
    });

    await analyticsPage.reset();

    await resetRequestPromise;

    await expect(analyticsPage.categorySelect).toHaveValue("");

    await expect(analyticsPage.startDateInput).toHaveValue("");

    await expect(analyticsPage.endDateInput).toHaveValue("");

    await expect(analyticsPage.minAmountInput).toHaveValue("");

    await expect(analyticsPage.maxAmountInput).toHaveValue("");

    await expect(analyticsPage.notesInput).toHaveValue("");
  });

  test("should reject start date after end date", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const requestTracker = await trackAnalyticsRequests(page);

    const initialCount = requestTracker.getCount();

    await analyticsPage.applyFilters({
      startDate: "2026-08-20",
      endDate: "2026-08-01",
    });

    await expect(
      page.getByText("Start date cannot be after end date."),
    ).toBeVisible();

    expect(requestTracker.getCount()).toBe(initialCount);
  });

  test("should reject negative minimum amount", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const requestTracker = await trackAnalyticsRequests(page);

    const initialCount = requestTracker.getCount();

    await analyticsPage.applyFilters({
      minAmount: "-100",
    });

    await expect(
      page.getByText("Minimum amount cannot be negative."),
    ).toBeVisible();

    expect(requestTracker.getCount()).toBe(initialCount);
  });

  test("should reject negative maximum amount", async ({ page }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const requestTracker = await trackAnalyticsRequests(page);

    const initialCount = requestTracker.getCount();

    await analyticsPage.applyFilters({
      maxAmount: "-100",
    });

    await expect(
      page.getByText("Maximum amount cannot be negative."),
    ).toBeVisible();

    expect(requestTracker.getCount()).toBe(initialCount);
  });

  test("should reject minimum amount greater than maximum amount", async ({
    page,
  }) => {
    await mockAnalyticsResponse(page);

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    const requestTracker = await trackAnalyticsRequests(page);

    const initialCount = requestTracker.getCount();

    await analyticsPage.applyFilters({
      minAmount: "1000",
      maxAmount: "100",
    });

    await expect(
      page.getByText("Minimum amount cannot be greater than maximum amount."),
    ).toBeVisible();

    expect(requestTracker.getCount()).toBe(initialCount);
  });

  test("should show empty state when analytics are unavailable", async ({
    page,
  }) => {
    await mockAnalyticsResponse(page, {
      success: true,
      message: "No analytics available",
      errorCode: null,
      data: {
        items: [],
        pageNumber: 1,
        pageSize: 10,
        totalRecords: 0,
        totalPages: 1,
        analytics: null,
      },
    });

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    await expect(analyticsPage.emptyState.first()).toBeVisible();

    await expect(analyticsPage.kpiCards).toHaveCount(0);
  });

  test("should show error toast when analytics API fails", async ({ page }) => {
    await page.route("**/expenses/filter-paged", async (route) => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({
          success: false,
          message: "Unable to load analytics.",
          errorCode: "SERVER_ERROR",
          data: null,
        }),
      });
    });

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    await expect(page.getByText("Unable to load analytics.")).toBeVisible();

    await expect(analyticsPage.emptyState.first()).toBeVisible();
  });

  test("should clear loading state after API failure", async ({ page }) => {
    await page.route("**/expenses/filter-paged", async (route) => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({
          message: "Server Error",
        }),
      });
    });

    const analyticsPage = new AnalyticsPage(page);

    await analyticsPage.open();

    await expect(analyticsPage.loadingIndicator).toBeHidden();

    await expect(analyticsPage.searchButton).toBeEnabled();

    await expect(analyticsPage.resetButton).toBeEnabled();
  });
});

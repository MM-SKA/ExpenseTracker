/// <reference types="node" />
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',

  fullyParallel: true,

  forbidOnly: Boolean(process.env['CI']),

  retries: process.env['CI'] ? 2 : 0,

  workers: process.env['CI'] ? 1 : undefined,

  reporter: [
    ['html', { open: 'never' }],
    ['list']
  ],

  use: {
    baseURL: 'http://localhost:4200',

    trace: 'on-first-retry',

    screenshot: 'only-on-failure',

    video: 'retain-on-failure',

    ignoreHTTPSErrors: true
  },

  projects: [
    {
      name: 'chromium',

      use: {
        ...devices['Desktop Chrome']
      }
    }
  ],

  // webServer: [
  //   {
  //     command: 'npm start',

  //     cwd: './finance-dashboard-ui',

  //     url: 'http://localhost:4200',

  //     reuseExistingServer: !process.env['CI'],

  //     timeout: 120_000
  //   },

  //   {
  //     command: 'dotnet run',

  //     cwd: './Finance.Api',

  //     url: 'http://localhost:5088',

  //     reuseExistingServer: !process.env['CI'],

  //     timeout: 15_000,

  //     ignoreHTTPSErrors: true
  //   }
  // ]
});

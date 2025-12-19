import { defineConfig, devices } from '@playwright/test';

/**
 * Playwright Configuration for Alfred Bot
 *
 * Usage:
 * - `npm run e2e` - Run with auto-started Angular dev server (frontend only)
 * - `npm run e2e:aspire` - Run against Aspire-managed stack (start Aspire first)
 * - `npm run e2e:ui` - Interactive UI mode
 * - `npm run e2e:headed` - Run with visible browser
 *
 * For full-stack testing with backend:
 * 1. Start Aspire: `dotnet run --project ../Alfred.AppHost`
 * 2. Run tests: `npm run e2e:aspire`
 */

const useAspire = process.env['USE_ASPIRE'] === 'true';
const backendUrl = process.env['BACKEND_URL'] || 'https://localhost:7170';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env['CI'],
  retries: process.env['CI'] ? 2 : 0,
  workers: process.env['CI'] ? 1 : undefined,
  reporter: [
    ['html'],
    ['list'],
  ],
  use: {
    baseURL: 'http://localhost:4200',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'on-first-retry',
    // Ignore HTTPS errors for local development with self-signed certs
    ignoreHTTPSErrors: true,
  },
  // Global timeout for each test
  timeout: 30000,
  // Expect timeout
  expect: {
    timeout: 5000,
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
  ],
  // Only start the web server if not using Aspire (Aspire manages everything)
  webServer: useAspire ? undefined : {
    command: 'npm run start',
    url: 'http://localhost:4200',
    reuseExistingServer: !process.env['CI'],
    timeout: 120000,
    stdout: 'pipe',
    stderr: 'pipe',
  },
  // Global setup/teardown (optional)
  // globalSetup: './e2e/global-setup.ts',
  // globalTeardown: './e2e/global-teardown.ts',
});

// Export for use in tests
export { backendUrl };

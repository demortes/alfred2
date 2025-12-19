import { test as base, expect } from '@playwright/test';

// Backend URL from environment or default
const BACKEND_URL = process.env['BACKEND_URL'] || 'https://localhost:7170';

/**
 * Extended test fixtures for Alfred Bot e2e tests
 */
export const test = base.extend<{
  /** Wait for backend to be healthy before running test */
  waitForBackend: void;
}>({
  waitForBackend: [async ({ request }, use) => {
    // Only wait for backend if USE_ASPIRE is set (running full stack)
    if (process.env['USE_ASPIRE'] === 'true') {
      const maxAttempts = 30;
      const delayMs = 1000;

      for (let attempt = 1; attempt <= maxAttempts; attempt++) {
        try {
          const response = await request.get(`${BACKEND_URL}/health`, {
            timeout: 5000,
            ignoreHTTPSErrors: true,
          });
          if (response.ok()) {
            console.log(`Backend healthy after ${attempt} attempt(s)`);
            break;
          }
        } catch {
          if (attempt === maxAttempts) {
            throw new Error(`Backend not healthy after ${maxAttempts} attempts`);
          }
          console.log(`Waiting for backend... (attempt ${attempt}/${maxAttempts})`);
          await new Promise(resolve => setTimeout(resolve, delayMs));
        }
      }
    }
    await use();
  }, { auto: false }],
});

export { expect };

/**
 * Helper to check if a service is healthy
 */
export async function isServiceHealthy(
  request: typeof base.request,
  url: string
): Promise<boolean> {
  try {
    const response = await request.get(url, {
      timeout: 5000,
      ignoreHTTPSErrors: true,
    });
    return response.ok();
  } catch {
    return false;
  }
}

/**
 * Backend API URL for direct API calls in tests
 */
export const backendUrl = BACKEND_URL;

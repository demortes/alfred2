import { test, expect, backendUrl } from './fixtures';

/**
 * Full-stack tests that require the backend to be running.
 * Run these with: npm run e2e:aspire (after starting Aspire)
 */
test.describe('Backend Integration', () => {
  // Skip these tests if not running with Aspire
  test.skip(
    () => process.env['USE_ASPIRE'] !== 'true',
    'Skipping backend tests - run with npm run e2e:aspire'
  );

  test.beforeEach(async ({ waitForBackend }) => {
    // Wait for backend to be healthy before each test
    await waitForBackend;
  });

  test('backend health check should return OK', async ({ request }) => {
    const response = await request.get(`${backendUrl}/health`, {
      ignoreHTTPSErrors: true,
    });
    expect(response.ok()).toBeTruthy();
  });

  test('bot settings endpoint should require authentication', async ({ request }) => {
    const response = await request.get(`${backendUrl}/api/botsettings`, {
      ignoreHTTPSErrors: true,
    });
    // Should return 401 Unauthorized without auth token
    expect(response.status()).toBe(401);
  });
});

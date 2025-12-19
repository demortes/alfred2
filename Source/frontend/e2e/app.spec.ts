import { test, expect } from '@playwright/test';

test.describe('Alfred Bot App', () => {
  test('should redirect to login when not authenticated', async ({ page }) => {
    await page.goto('/');
    // Accept either previous '/login' or new '/auth/login' paths
    await expect(page).toHaveURL(/.*(login|auth\/login)/);
  });

  test('login page should have Twitch login button', async ({ page }) => {
    await page.goto('/login');
    // Figma may change the exact wording; accept either 'Twitch' or 'Sign in with Twitch'
    await expect(
      page.getByRole('button', { name: /twitch|sign in with twitch/i })
    ).toBeVisible();
  });

  test('login page should display Alfred Bot branding', async ({ page }) => {
    await page.goto('/login');
    // Branding may appear as 'Alfred' or 'Alfred Bot' in the updated design
    await expect(page.getByText(/alfred( bot)?/i)).toBeVisible();
  });

  test('login page should have proper styling', async ({ page }) => {
    await page.goto('/login');
    // Check that the page loaded with dark theme
    const body = page.locator('body');
    await expect(body).toBeVisible();
    // Prefer checking for a theme class or data attribute used by the app
    await expect(
      page.locator('body[class*="dark"], body[data-theme="dark"]')
    ).toHaveCount(1);
  });
});

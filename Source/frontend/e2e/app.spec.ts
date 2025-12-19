import { test, expect } from '@playwright/test';

test.describe('Alfred Bot App', () => {
  test('should redirect to login when not authenticated', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveURL(/.*login/);
  });

  test('login page should have Twitch login button', async ({ page }) => {
    await page.goto('/login');
    await expect(page.getByRole('button', { name: /twitch/i })).toBeVisible();
  });

  test('login page should display Alfred Bot branding', async ({ page }) => {
    await page.goto('/login');
    await expect(page.getByText('Alfred Bot')).toBeVisible();
  });
});

from playwright.sync_api import sync_playwright

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()

    # Navigate to the login page and take a screenshot
    page.goto("http://localhost:4200/login")
    page.screenshot(path="jules-scratch/verification/login-page.png")

    # Navigate to the dashboard and take a screenshot
    page.goto("http://localhost:4200/dashboard")
    page.screenshot(path="jules-scratch/verification/dashboard-page-unauthenticated.png")

    # Since we can't log in via Twitch, we'll manually set the token
    # to simulate an authenticated state and access the dashboard.
    page.evaluate("localStorage.setItem('auth_token', 'fake_token_for_testing')")
    page.goto("http://localhost:4200/dashboard")
    page.screenshot(path="jules-scratch/verification/dashboard-page-authenticated.png")

    browser.close()

with sync_playwright() as playwright:
    run(playwright)
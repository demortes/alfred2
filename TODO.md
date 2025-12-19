# Alfred (alfred2) — Project TODO

This file captures high-level tasks, priorities, and recommended next steps for contributors working on the repository.

## Priority (High)
- Add example environment files
  - Create `Source/frontend/.env.example` and `Source/frontend/src/environments/.env.example` with the keys: `PRODUCTION`, `API_URL`, `TWITCH_CLIENT_ID`, `BACKEND_URL`, etc.
  - Create `Source/AlfredBackend/.env.example` with DB and auth placeholders.
- Harden CI for missing env files
  - Ensure workflows provide safe defaults or read from `.env.example`.
- Validate and run Playwright e2e tests
  - Run `npx playwright test e2e` (or `npm run e2e`) and fix failures.
  - Prefer stable selectors (data-testid) in the app to avoid brittle tests.
- Verify Aspire & backend integration
  - Run `aspire run` and confirm all resources start healthy before running full-stack tests.

## Improvements (Medium)
- Add `.env.example` to repo and list required env variables in README.
- Add contributing guide with local dev setup steps:
  - `aspire run` instructions
  - `cd Source/frontend && npm ci && npm start`
  - How to run Playwright tests (headed/CI modes)
- Make Playwright tests resilient:
  - Use `data-testid` attributes for important elements
  - Avoid brittle text-based matches where possible
- Add pre-commit hooks: lint, format, and basic test run

## Tech debt / Housekeeping (Low)
- Review committed `playwright-report/` artifacts — consider ignoring generated reports in `.gitignore`.
- Update dependencies regularly; run `npm audit` and `dotnet list package --outdated` when updating.
- Add CI job to run e2e tests in a controlled environment (use Aspire or mock backend).

## Suggested Next Actions (for me)
- Create `.env.example` files and open a PR.
- Run Playwright tests locally and fix any remaining failures.
- Add a short CONTRIBUTING.md with dev setup steps.

## Useful Commands
- Frontend dev:
```bash
cd Source/frontend
npm ci
npm start
```
- Run Playwright tests:
```bash
cd Source/frontend
npx playwright test
```
- Run Aspire apphost:
```bash
aspire run
```

If you want, I can create the `.env.example` files and a short `CONTRIBUTING.md` next — would you like me to proceed with that?
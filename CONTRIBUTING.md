# Contributing to Alfred (alfred2)

Thanks for your interest in contributing! This guide covers local setup, testing, and the preferred workflow for changes.

## Quick start (developer)

1. Fork and clone the repo:

```bash
git clone https://github.com/<your-user>/alfred2.git
cd alfred2
```

2. Copy example environment files and fill values:

```bash
# Frontend
cp Source/frontend/.env.example Source/frontend/.env
# Backend
cp Source/AlfredBackend/.env.example Source/AlfredBackend/.env
```

3. Start Aspire (recommended) to bring up app resources:

```bash
aspire run
```

4. Frontend

```bash
cd Source/frontend
npm ci
npm start
```

The frontend `config` step (run automatically by `npm start` and `npm run build`) generates `src/environments/environment.ts` from your environment variables or `.env` file.

5. Backend

Open `Source/AlfredBackend` in Visual Studio / VS Code and run/debug as usual or run `dotnet run` from CLI. Ensure your `ConnectionStrings` and other sensitive values are set in `Source/AlfredBackend/.env` or environment variables.

## Running tests

- Unit tests (backend):

```bash
cd Source
dotnet test
```

- Frontend unit tests:

```bash
cd Source/frontend
npm test
```

- Playwright end-to-end tests:

```bash
cd Source/frontend
# For local e2e (uses local backend or Aspire endpoints)
npx playwright test
# For Aspire-backed full-stack tests
npm run e2e:aspire
```

Notes: The Playwright config reads `BACKEND_URL` and `USE_ASPIRE` from environment variables. Use `.env` to set these for local runs or use `cross-env` in npm scripts.

## Environment files

Example files have been added to the repository:

- `Source/frontend/.env.example`
- `Source/frontend/src/environments/.env.example`
- `Source/AlfredBackend/.env.example`

These files contain the keys required by the application. Do NOT commit your filled `.env` files — they are ignored by `.gitignore`.

## CI and GitHub Actions

The frontend CI workflow includes safe defaults for missing env vars so builds do not fail when `.env` files are absent. If you add new required env keys, update the workflow or include them in the example files.

## Code style and pre-commit

Consider adding pre-commit hooks for linting and formatting. A sample setup:

- `husky` for hooks
- `eslint` / `prettier` for formatting

## Pull requests

- Fork the repo and create a feature branch.
- Keep PRs small and focused.
- Include tests where applicable.
- Reference issues if available.

If you want, I can open a PR with the example env files and this `CONTRIBUTING.md`.

# Finance Dashboard UI

Angular frontend for the Personal Finance Dashboard.

## Overview

This project is the client application for managing user authentication, categories, and expenses.

## Features

- User registration and login
- JWT authentication
- Category creation and caching
- Expense creation and list display
- Expense filtering by text and date range
- Offline expense storage with local fallback

## Development

Install dependencies and start the local server:

```bash
npm install
npm start
```

Open the browser at `http://localhost:4200/`.

## Scripts

- `npm start` — run development server
- `npm test` — run unit tests
- `npm run build` — build the app
- `npm run watch` — build continuously during development
- `npm run serve:ssr:finance-dashboard-ui` — run SSR server build

## Local storage usage

The UI stores the following keys in browser `localStorage`:

- `token` — JWT token used for authenticated API requests
- `user` — authenticated user object stored as JSON
- `categories_cache` — cached category list used by `CategoryService`
- `expenses` — offline expense drafts or expenses saved locally when the API is unavailable or the user is not authenticated

## Notes

- The `authInterceptor` sends `Authorization: Bearer <token>` when the token exists.
- Local expense storage is used as a fallback when API calls fail or no token is present.
- Category caching helps reduce API requests and improve performance.

## Build

```bash
npm run build
```

This compiles the app and outputs artifacts to `dist/`.

## Testing

```bash
npm test
```

## Resources

For Angular CLI docs, see [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli).

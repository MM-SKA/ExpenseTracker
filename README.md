# Personal Finance Dashboard

A full-stack personal finance tracker consisting of an ASP.NET Core API and an Angular frontend.

## Project structure

- `Finance.Api/` - ASP.NET Core API with JWT authentication, categories, and expense management.
- `finance-dashboard-ui/` - Angular client application for login, category management, expense tracking, and analytics.

## Key features

- JWT-based authentication
- Category management
- Expense creation, listing, and filtering
- Local offline expense storage in the browser
- API + UI separation for easier maintenance

## Getting started

### Backend

1. Open a terminal in `Finance.Api/`
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Run the API:
   ```bash
   dotnet run
   ```

### Frontend

1. Open a terminal in `finance-dashboard-ui/`
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the app:
   ```bash
   npm start
   ```
4. Open the browser at `http://localhost:4200/`

## Local storage usage

The frontend stores the following browser data in `localStorage`:

- `token` — JWT authentication token set after login or registration
- `user` — authenticated user object serialized as JSON
- `categories_cache` — cached category data used by the category service
- `expenses` — locally stored expense drafts / fallback expense list when the API is unavailable or the user is not authenticated

> Note: `localStorage` data is only used by the Angular frontend.

## Notes

- The UI will use local expense storage when an API call fails or when the user is not authenticated.
- The API expects a valid JWT token in the `Authorization: Bearer <token>` header.
- `Finance.Api` and `finance-dashboard-ui` can be run independently during development.

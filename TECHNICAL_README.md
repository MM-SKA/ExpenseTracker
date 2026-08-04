# Technical README

This technical README describes the architecture and storage behavior of the Personal Finance Dashboard project.

## Architecture Overview

The project is divided into two primary applications:

- `Finance.Api/` — ASP.NET Core API project
- `finance-dashboard-ui/` — Angular client project

### Finance.Api

The backend includes:

- `Presentation/Controllers` — API controllers handling HTTP requests
- `Application/Services` — business logic services
- `Infrastructure/Data` — Entity Framework Core database context
- `Domain/Entities` — domain models: `AppUser`, `Category`, `Expense`
- `Presentation/Middleware` — request logging and exception handling middleware

### finance-dashboard-ui

The frontend includes:

- `src/app/features` — feature modules for auth, categories, expenses, analytics
- `src/app/core/services` — reusable services such as auth, category, expense
- `src/app/core/interceptors` — HTTP interceptor for JWT auth
- `src/app/shared/models` — data models and request/response DTOs

## Frontend local storage usage

The Angular app uses `localStorage` in the browser for the following values:

- `token`
  - Purpose: JWT token storage after login or registration
  - Used by: `auth-interceptor.ts` to attach an Authorization header
- `user`
  - Purpose: stores the authenticated user object as JSON
  - Used by: UI code that needs basic user info and session persistence
- `categories_cache`
  - Purpose: cache categories locally to reduce API calls
  - Used by: `CategoryService.getCategories()` and `CategoryService.clearCache()`
- `expenses`
  - Purpose: fallback local expense storage when API calls fail or user is unauthenticated
  - Used by: `CreateExpense.saveToLocal()` and `ExpenseList.load()`

### Expense storage behavior

- If the user is authenticated and the API call succeeds, expenses are created through the backend.
- If the user is unauthenticated or the expense API call fails, expense data is written to `localStorage` under `expenses`.
- The expense list page loads from the backend when authenticated, otherwise it reads `expenses` from `localStorage`.
- Deleting a locally stored expense updates the `expenses` key in `localStorage`.

## Recommended improvements

### Backend improvements

- Add full Swagger / OpenAPI documentation
- Improve error handling with global exception middleware
- Add validation using FluentValidation or data annotations
- Add structured logging using Serilog or similar
- Add pagination and filtering on server-side expense queries

### Frontend improvements

- Add offline sync for `localStorage` expenses when connection is restored
- Add UI controls for clearing cached categories and local expenses
- Add more robust error handling and user feedback for API failures
- Introduce typed models and service return types for stronger compile-time checks

## Running the project

### Backend

```bash
cd Finance.Api
dotnet restore
dotnet run
```

### Frontend

```bash
cd finance-dashboard-ui
npm install
npm start
```

## Environment configuration

The backend likely uses `appsettings.json` and `appsettings.Development.json` for configuration such as:

- Database connection string
- JWT secret, issuer, audience, expiration

The frontend uses `environment/environment.ts` for `apiUrl` configuration.

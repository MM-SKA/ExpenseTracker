# Personal Finance Dashboard API

A modern, RESTful API for managing personal finances built with ASP.NET Core, Entity Framework Core, and JWT authentication.

## 📋 Features

### ✅ Current Features
- **User Authentication** - JWT-based authentication with registration and login
- **Category Management** - Create, read, update, delete expense categories
- **Expense Tracking** - Full CRUD operations for expenses
- **User-Isolated Data** - Data is isolated per user for security
- **Structured API Responses** - Consistent response format across all endpoints
- **DTOs** - Complete separation between API contracts and database models
- **Duplicate Validation** - Prevents duplicate category names per user

### 🚀 Recommended Features to Add

#### Priority 1 (Essential for Production)
1. **Global Exception Handling** - Centralized error handling middleware
2. **Service Layer Architecture** - Separation of business logic from controllers
3. **Swagger/OpenAPI Documentation** - Auto-generated API documentation
4. **Logging** - Structured logging for debugging and monitoring
5. **Input Validation** - FluentValidation for robust data validation

#### Priority 2 (Core Finance Features)
1. **Analytics Endpoints** - Total spent, monthly trends, category breakdown
2. **Pagination** - Limit results for large datasets
3. **Expense Filtering & Sorting** - By date range, category, amount
4. **Budget Management** - Set and track budgets per category
5. **Expense Search** - Full-text search for expenses by notes

#### Priority 3 (Enhanced Features)
1. **Recurring Expenses** - Set up monthly/yearly recurring expenses
2. **Data Export** - Export expenses as CSV/PDF
3. **Spending Insights** - Top spending categories, trends
4. **Rate Limiting** - Prevent API abuse
5. **Caching** - Redis caching for analytics queries

#### Priority 4 (User Management)
1. **User Profile Updates** - Update full name, email
2. **Password Change** - Secure password update endpoint
3. **Account Deletion** - GDPR compliance
4. **Email Verification** - Confirm email on registration
5. **Password Reset** - Forgot password functionality

---

## 🏗️ Architecture

### Current Structure
```
Controllers (HTTP Layer)
    ├── AuthController
    ├── CategoriesController
    └── ExpensesController
    
Models (Database)
    ├── AppUser
    ├── Category
    └── Expense
    
DTOs (API Contracts)
    ├── Auth
    ├── Category
    └── Expenses
```

### Recommended Structure (After Refactoring)
```
Controllers (thin, HTTP only)
    ↓
Services (business logic)
    ├── IAuthService
    ├── ICategoryService
    └── IExpenseService
    ↓
Repositories (data access)
    ├── IRepository<T>
    └── Unit of Work
    ↓
Models (database)
    
Middleware
    ├── ExceptionHandlingMiddleware
    ├── LoggingMiddleware
    └── RateLimitingMiddleware
```

---

## 📚 API Endpoints

### Authentication
```
POST   /api/auth/register      - Register new user
POST   /api/auth/login         - Login user
GET    /api/auth/me            - Get current user
GET    /api/auth/users         - List all users (admin)
```

### Categories
```
POST   /api/categories         - Create category
GET    /api/categories         - List user's categories
PUT    /api/categories/{id}    - Update category
DELETE /api/categories/{id}    - Delete category
```

### Expenses
```
POST   /api/expense/create             - Create expense
GET    /api/expense/get                - List user's expenses
PUT    /api/expense/{id}               - Update expense
DELETE /api/expense/delete/{id}        - Delete expense
```

### Analytics (Planned)
```
GET    /api/analytics/summary           - Total spent, monthly stats
GET    /api/analytics/by-category       - Breakdown by category
GET    /api/analytics/trends            - Spending trends
GET    /api/analytics/monthly           - Monthly comparison
```

---

## 🔐 Security Features

- ✅ JWT Authentication
- ✅ User data isolation
- ✅ Password hashing with BCrypt
- ✅ Authorization checks on all endpoints
- 🔄 CORS configuration (to implement)
- 🔄 Rate limiting (to implement)
- 🔄 SQL injection prevention (via EF Core)
- 🔄 HTTPS enforcement (to implement)

---

## 📊 Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {
    "id": 1,
    "name": "Food"
  }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Category with this name already exists"
}
```

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT
- **Password Hashing**: BCrypt.Net
- **API Documentation**: Swagger (planned)
- **Validation**: FluentValidation (planned)
- **Logging**: Serilog (planned)

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server
- Visual Studio or VS Code

### Installation
```bash
# Clone repository
git clone <repo-url>

# Install dependencies
dotnet restore

# Update database
dotnet ef database update

# Run API
dotnet run
```

### Environment Variables
Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FinanceDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "Secret": "your-secret-key-here-min-32-characters",
    "Issuer": "FinanceApi",
    "Audience": "FinanceApiUsers",
    "ExpirationMinutes": 60
  }
}
```

---

## 📝 Next Steps

1. **Implement Global Exception Handling** - Create middleware for centralized error handling
2. **Build Service Layer** - Extract business logic to services
3. **Add Analytics Endpoints** - Implement summary and trend calculations
4. **Add Swagger Documentation** - Auto-generate API docs
5. **Implement Unit Tests** - Test services and controllers
6. **Add Logging** - Structured logging with Serilog
7. **Input Validation** - Use FluentValidation
8. **API Versioning** - Prepare for v2

---

## 📄 License

MIT License - feel free to use this project as a template.

---

## 👨‍💻 Development Notes

### Code Quality
- ✅ DTOs for all endpoints
- ✅ Consistent error responses
- ✅ User data isolation
- ✅ Validation on creation
- 🔄 Unit tests needed
- 🔄 Integration tests needed

### Performance
- ✅ AsNoTracking() on GET endpoints
- 🔄 Query optimization needed
- 🔄 Caching strategy needed
- 🔄 Pagination needed for large datasets

### Security
- ✅ JWT authentication
- ✅ Password hashing
- 🔄 CORS configuration needed
- 🔄 Rate limiting needed
- 🔄 HTTPS enforcement needed

---

**Last Updated**: 2026-07-06

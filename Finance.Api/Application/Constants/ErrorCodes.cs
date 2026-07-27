namespace Finance.Api.Application.Constants;

public static class ErrorCodes
{
    // Auth
    public const string InvalidCredentials = "AUTH002";
    public const string UserNotFound = "AUTH003";
    public const string NewPasswordMatchesOldPassword = "AUTH005";
    public const string InvalidPasswordWhileChangingPassword = "AUTH007";
    public const string DuplicateRequest = "AUTH008";

    // Categories
    public const string DuplicateCategory = "CAT001";
    public const string CategoryNotFound = "CAT002";
    public const string SystemCategoryUpdate = "CAT003";

    // Expenses
    public const string ExpenseNotFound = "EXP001";
    public const string InvalidCategory = "EXP002";

    // Database
    public const string DatabaseConnectionFailed = "DB001";
    public const string DatabaseDnsResolutionFailed = "DB002";
    public const string DatabaseTimeout = "DB003";
    public const string DatabaseUnavailable = "DB004";

}

using Microsoft.Extensions.Logging;

namespace Finance.Api.Application.Logs.AppUser;

internal static partial class AppUserServiceLogs
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "User {Email} logged in")]
    public static partial void UserLogin(
        ILogger logger, string Email);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "New User Registered {Email}")]
    public static partial void UserRegisters(ILogger logger, string Email);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "Invalid Credentials Attempt for {Email}"
    )]
    public static partial void InvalidAttempt(ILogger logger, string Email);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "Password Updated Successfully for {Email}"
    )]
    public static partial void PasswordUpdateSuccess(ILogger logger, string Email);
}

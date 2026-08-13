using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Quartz;

using Finance.Api.Application.Interfaces;
using Finance.Api.Application.Interfaces.V1;
using Finance.Api.Application.Interfaces.V2;
using Finance.Api.Application.Services;
using Finance.Api.Application.Services.V1;
using Finance.Api.Application.Services.V2;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Services;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Presentation.Middleware;
using Finance.Api.Infrastructure.Options;
using Finance.Api.Infrastructure.Repositories;
using Finance.Api.Infrastructure.Jobs;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Finance.Api.Application.DTOs.V1.Expenses;
using Finance.Api.Application.DTOs.V2.Expenses;

using FluentValidation;
using Finance.Api.Application.Validation.User;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging(options => { });

builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseServiceV1<ExpenseDtoV1>, ExpenseServiceV1>();
builder.Services.AddScoped<IExpenseServiceV2<ExpenseDtoV2>, ExpenseServiceV2>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAiExpenseService, AiExpenseService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.Configure<GroqOptions>(builder.Configuration.GetSection("Groq"));
builder.Services.AddHttpClient<IGroqService, GroqService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };

        options.Events =
            new JwtBearerEvents
            {
                OnMessageReceived =
                    context =>
                    {
                        context.Token =
                            context.Request
                                .Cookies["accessToken"];

                        return Task.CompletedTask;
                    }
            };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Angular",
        policy =>
        {
            _ = policy
                .WithOrigins(
                    "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddQuartz(quartz =>
{
    var jobKey =
        new JobKey(
            "RefreshTokenCleanup");

    _ = quartz.AddJob<RefreshTokenCleanupJob>(
        options =>
            options.WithIdentity(jobKey));

    _ = quartz.AddTrigger(trigger =>
        trigger
            .ForJob(jobKey)
            .WithIdentity(
                "RefreshTokenCleanupTrigger")
            .WithCronSchedule(
                "0 0 2 * * ?"));
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("api-version")
    );
}).AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention())).AddApiExplorer(options =>
{
    options.GroupNameFormat = "v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<FinanceDbContext>(
    options =>
        options.UseCosmos(
            builder.Configuration["CosmosDb:AccountEndpoint"]!,
            builder.Configuration["CosmosDb:AccountKey"]!,
            builder.Configuration["CosmosDb:DatabaseName"]!,
            cosmosOptions => cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway)
            ));

builder.Services.AddQuartzHostedService(
    options =>
    {
        options.WaitForJobsToComplete =
            true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // _ = app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<FinanceDbContext>();

    _ = await context.Database
        .EnsureCreatedAsync()
        .ConfigureAwait(false);

    var existingSystemCategories =
        await context.Categories
            .Where(c => c.IsSystemCategory)
            .OrderBy(c => c.Id)
            .Take(1)
            .ToListAsync()
            .ConfigureAwait(false);

    if (existingSystemCategories.Count == 0)
    {
        context.Categories.AddRange(
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                IsSystemCategory = true,
                UserId = "System"
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Travel",
                IsSystemCategory = true,
                UserId = "System"
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entertainment",
                IsSystemCategory = true,
                UserId = "System"
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Shopping",
                IsSystemCategory = true,
                UserId = "System"
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Health",
                IsSystemCategory = true,
                UserId = "System"
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Education",
                IsSystemCategory = true,
                UserId = "System"
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bills",
                IsSystemCategory = true,
                UserId = "System"
            });

        _ = await context.SaveChangesAsync()
            .ConfigureAwait(false);
    }
}

// app.UseHttpsRedirection();
// app.UseHttpLogging();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

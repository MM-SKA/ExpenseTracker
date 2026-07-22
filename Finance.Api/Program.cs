using Finance.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.Interfaces;
using Finance.Api.Application.Interfaces.V1;
using Finance.Api.Application.Interfaces.V2;
using Finance.Api.Application.Services;
using Finance.Api.Application.Services.V1;
using Finance.Api.Application.Services.V2;
using Finance.Api.Infrastructure.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Finance.Api.Domain.Entities;
using Finance.Api.Presentation.Middleware;
using Finance.Api.Application.DTOs.V1.Expenses;
using Asp.Versioning;
using Finance.Api.Application.DTOs.V2.Expenses;
using Asp.Versioning.Conventions;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Log.Logger = new LoggerConfiguration()
//     .WriteTo.Console()
//     .WriteTo.File(
//         "logs/log-.txt",
//         rollingInterval: RollingInterval.Day)
//     .CreateLogger();

// builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpLogging(options => { });

builder.Services.AddScoped<IJWTService, JWTService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IExpenseServiceV1<ExpenseDtoV1>, ExpenseServiceV1>();
builder.Services.AddScoped<IExpenseServiceV2<ExpenseDtoV2>, ExpenseServiceV2>();

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
    });

builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
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

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<FinanceDbContext>();

    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category
            {
                Name = "Food",
                IsSystemCategory = true
            },
            new Category
            {
                Name = "Travel",
                IsSystemCategory = true
            },
            new Category
            {
                Name = "Entertainment",
                IsSystemCategory = true
            },
            new Category
            {
                Name = "Shopping",
                IsSystemCategory = true
            },
            new Category
            {
                Name = "Health",
                IsSystemCategory = true
            },
            new Category
            {
                Name = "Education",
                IsSystemCategory = true
            },
            new Category
            {
                Name = "Bills",
                IsSystemCategory = true
            }
        );

        _ = context.SaveChanges();
    }
}

// app.UseHttpsRedirection();
// app.UseHttpLogging();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

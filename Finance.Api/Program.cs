using Finance.Api.Data;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.Interfaces;
using Finance.Api.Application.Services;
using Finance.Api.Infrastructure.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Finance.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(options =>{});


builder.Services.AddScoped<IJWTService,JWTService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IExpenseAnalyticsService, ExpenseAnalyticsService>();

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

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using(var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<FinanceDbContext>();

    if(!context.Categories.Any())
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

        context.SaveChanges();
    }
}

// app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
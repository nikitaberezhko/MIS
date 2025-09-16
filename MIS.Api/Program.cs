using Microsoft.EntityFrameworkCore;
using MIS.Api.Infrastructure;
using MIS.ServiceDefaults;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MIS.Api.Infrastructure.Repositories;
using MIS.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Регистрация ExceptionHandlerMiddleware
builder.Services.AddScoped<ExceptionHandlerMiddleware>();

// Настройка Swagger для development environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        
        
            // Fallback если файл не найден
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Medical Information System (MIS) API",
                Version = "v1",
                Description = "API для медицинской информационной системы"
            });
        
        
        // Добавляем XML комментарии если есть
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });
}

// Настройка базы данных в зависимости от окружения
if (builder.Environment.IsEnvironment("Testing"))
{
    // Для тестового окружения используем прямую настройку DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("mis-db") 
            ?? builder.Configuration["Aspire:Npgsql:EntityFrameworkCore:PostgreSQL:ConnectionString"]
            ?? builder.Configuration["Aspire:Npgsql:EntityFrameworkCore:PostgreSQL:AppDbContext:ConnectionString"];
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("ConnectionString is missing. It should be provided in 'ConnectionStrings:mis-db' or under the 'ConnectionString' key in 'Aspire:Npgsql:EntityFrameworkCore:PostgreSQL' or 'Aspire:Npgsql:EntityFrameworkCore:PostgreSQL:AppDbContext' configuration section.");
        }
        
        options.UseNpgsql(connectionString);
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    });
}
else
{
    // Для остальных окружений используем Aspire
    builder.AddNpgsqlDbContext<AppDbContext>("mis-db");
}

builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<DiseaseRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Настройка Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MIS API (from openapi.yml)");
        c.RoutePrefix = "swagger"; // Swagger UI будет доступен по адресу /swagger
        c.DocumentTitle = "MIS API Documentation";
    });
}

app.MapDefaultEndpoints();

app.UseMiddleware<ExceptionHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapGet("/health", () => Results.Ok("OK"));

app.MapControllers();

app.Run();

public partial class Program { }
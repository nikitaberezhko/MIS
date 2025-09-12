using Aspire.Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MIS.Api.Infrastructure;
using MIS.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

// Register AppDbContext using Aspire Npgsql. The connection comes from AppHost resource "mis-db".
builder.AddNpgsqlDbContext<AppDbContext>("mis-db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapGet("/healthz", () => Results.Ok("OK"));

// Map domain endpoints
app.MapPatients();
app.MapDoctors();
app.MapDiseases();
app.MapPatientDiseases();

app.Run();

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using MIS.Api.Infrastructure;

namespace IntegrationTests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    private WebApplicationFactory<Program>? _factory;
    private IServiceScope? _scope;
    
    protected HttpClient Client { get; private set; } = null!;
    protected AppDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(5432, true)
            .Build();

        await _postgresContainer.StartAsync();
        
        var connectionString = _postgresContainer.GetConnectionString();
        
        Environment.SetEnvironmentVariable("ConnectionStrings__mis-db", connectionString);
        Environment.SetEnvironmentVariable("Aspire__Npgsql__EntityFrameworkCore__PostgreSQL__ConnectionString", connectionString);
        Environment.SetEnvironmentVariable("Aspire__Npgsql__EntityFrameworkCore__PostgreSQL__AppDbContext__ConnectionString", connectionString);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "");
        
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);
                    
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseNpgsql(connectionString);
                        options.EnableSensitiveDataLogging();
                        options.EnableDetailedErrors();
                    });
                    
                    services.AddLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Warning);
                    });
                });
            });

        Client = _factory.CreateClient();
        
        _scope = _factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        _scope?.Dispose();
        Client?.Dispose();
        _factory?.Dispose();
        
        if (_postgresContainer != null)
        {
            await _postgresContainer.StopAsync();
            await _postgresContainer.DisposeAsync();
        }
        
        Environment.SetEnvironmentVariable("ConnectionStrings__mis-db", null);
        Environment.SetEnvironmentVariable("Aspire__Npgsql__EntityFrameworkCore__PostgreSQL__ConnectionString", null);
        Environment.SetEnvironmentVariable("Aspire__Npgsql__EntityFrameworkCore__PostgreSQL__AppDbContext__ConnectionString", null);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);
    }
    
    protected async Task ClearDatabaseAsync()
    {
        DbContext.Diseases.RemoveRange(DbContext.Diseases);
        DbContext.Doctors.RemoveRange(DbContext.Doctors);
        DbContext.Patients.RemoveRange(DbContext.Patients);
        
        await DbContext.SaveChangesAsync();
    }
}
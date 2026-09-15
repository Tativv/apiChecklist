using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace HotelChecklist.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("hotelchecklist_test")
        .WithUsername("hotelchecklist")
        .WithPassword("hotelchecklist_test")
        .Build();

    private readonly string _uploadsRoot = Path.Combine(Path.GetTempPath(), "hotelchecklist-tests", Guid.NewGuid().ToString());

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _dbContainer.GetConnectionString(),
                ["SeedOnStartup"] = "true",
                ["Jwt:Issuer"] = "HotelChecklist",
                ["Jwt:Audience"] = "HotelChecklist.Clients",
                ["Jwt:SigningKey"] = "integration-test-signing-key-not-for-production-use-1234567890",
                ["Jwt:ExpiryMinutes"] = "60",
                ["FileStorage:RootPath"] = _uploadsRoot
            });
        });
    }

    public Task InitializeAsync() => _dbContainer.StartAsync();

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();

        if (Directory.Exists(_uploadsRoot))
            Directory.Delete(_uploadsRoot, recursive: true);

        await base.DisposeAsync();
    }
}

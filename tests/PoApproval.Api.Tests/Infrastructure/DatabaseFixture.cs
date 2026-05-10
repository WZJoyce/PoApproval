using Microsoft.EntityFrameworkCore;
using PoApproval.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace PoApproval.Api.Tests.Infrastructure;

/// <summary>
/// xUnit fixture that spins up a SQL Server container for the duration of a test class.
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Apply migrations on container startup so each test class gets a fresh schema.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.MigrateAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    /// <summary>
    /// Resets all data while preserving schema. Call from test base class before each test.
    /// </summary>
    public async Task ResetAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.ExecuteSqlRawAsync("DELETE FROM PurchaseOrders");
    }
}

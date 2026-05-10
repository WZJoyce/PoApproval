namespace PoApproval.Api.Tests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    protected DatabaseFixture Database { get; }
    protected IntegrationTestFactory Factory { get; }

    protected IntegrationTestBase(DatabaseFixture database)
    {
        Database = database;
        Factory = new IntegrationTestFactory(database.ConnectionString);
    }

    public Task InitializeAsync() => Database.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}

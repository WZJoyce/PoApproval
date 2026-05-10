namespace PoApproval.Domain.Repositories;

/// <summary>
/// Coordinates persistence of changes tracked across one or more repositories
/// within a single business transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits all pending changes to the underlying store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

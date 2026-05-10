using PoApproval.Domain.Entities;

namespace PoApproval.Domain.Repositories;

/// <summary>
/// Repository for managing PurchaseOrder persistence.
/// </summary>
public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrder?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);

    Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PurchaseOrder>> ListAsync(CancellationToken cancellationToken = default);
}

using PoApproval.Domain.Advisory;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;

namespace PoApproval.Domain.Repositories;

/// <summary>
/// Repository for managing PurchaseOrder persistence.
/// </summary>
public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrder?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);

    Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default);

    Task<PagedResult<PurchaseOrder>> ListAsync(PurchaseOrderStatus? statusFilter, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<RequesterHistory> GetRequesterHistoryAsync(string requester, int excludeOrderId, CancellationToken cancellationToken = default);
}

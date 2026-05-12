using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Repositories;

namespace PoApproval.Domain.Services;

public interface IOrderQueryService
{
    Task<PurchaseOrder> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<PurchaseOrder>> ListAsync(
        PurchaseOrderStatus? statusFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

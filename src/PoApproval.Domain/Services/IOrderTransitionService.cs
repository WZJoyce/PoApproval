using PoApproval.Domain.Entities;

namespace PoApproval.Domain.Services;

public interface IOrderTransitionService
{
    Task<PurchaseOrder> SubmitAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrder> ApproveAsync(int id, string approver, CancellationToken cancellationToken = default);

    Task<PurchaseOrder> RejectAsync(int id, string approver, string reason, CancellationToken cancellationToken = default);
}

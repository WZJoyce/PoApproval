using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;

namespace PoApproval.Domain.Services;

public sealed class ApprovalService : IApprovalService
{
    private readonly IClock _clock;

    public ApprovalService(IClock clock)
    {
        _clock = clock;
    }

    public void Submit(PurchaseOrder order)
    {
        if (order.Status != PurchaseOrderStatus.Draft)
        {
            throw new InvalidStateTransitionException(
                order.Status,
                PurchaseOrderStatus.Submitted);
        }

        order.Status = PurchaseOrderStatus.Submitted;
    }

    public void Approve(PurchaseOrder order, string approver)
    {
        throw new NotImplementedException();
    }

    public void Reject(PurchaseOrder order, string approver, string reason)
    {
        throw new NotImplementedException();
    }
}
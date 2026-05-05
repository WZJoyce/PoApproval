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
        EnsureCurrentStatus(order, PurchaseOrderStatus.Draft, PurchaseOrderStatus.Submitted);

        order.Status = PurchaseOrderStatus.Submitted;
    }

    public void Approve(PurchaseOrder order, string approver)
    {

        EnsurenonNullOrWhitespace(approver, "APPROVER_REQUIRED", "Approver is required.");

        EnsureCurrentStatus(order, PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Approved);

        EnsureNotSelApproval(order, approver);

        order.Status = PurchaseOrderStatus.Approved;
        order.ApprovedBy = approver;
        order.ApprovedAt = _clock.UtcNow;
    }

    public void Reject(PurchaseOrder order, string approver, string reason)
    {
        throw new NotImplementedException();
    }

    private static void EnsureCurrentStatus(PurchaseOrder order, PurchaseOrderStatus required, PurchaseOrderStatus targetForError)
    {
        if (order.Status != required)
        {
            throw new InvalidStateTransitionException(order.Status, targetForError);
        }
    }

    private static void EnsurenonNullOrWhitespace(string value, string ruleCode, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleViolationException(
                ruleCode, message);
        }
    }

    private static void EnsureNotSelApproval(PurchaseOrder order, string approver)
    {
        if (string.Equals(order.CreatedBy, approver, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleViolationException(
              "SELF_APPROVAL_FORBIDDEN",
                "An order cannot be approved by the user who created it.");
        }
    }

}

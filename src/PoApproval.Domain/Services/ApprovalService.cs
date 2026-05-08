using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;

namespace PoApproval.Domain.Services;

public sealed class ApprovalService : IApprovalService
{
    private const int _minRejectionReasonLength = 10;

    private static readonly PurchaseOrderStatus[] _submittableStatuses = [
        PurchaseOrderStatus.Draft,
        PurchaseOrderStatus.Rejected

    ];

    private readonly IClock _clock;

    public ApprovalService(IClock clock)
    {
        _clock = clock;
    }

    public void Submit(PurchaseOrder order)
    {
        EnsureCurrentStatusIn(order, _submittableStatuses, PurchaseOrderStatus.Submitted);

        order.Status = PurchaseOrderStatus.Submitted;
        ClearReviewMetadata(order);
    }

    public void Approve(PurchaseOrder order, string approver)
    {
        EnsurenonNullOrWhitespace(approver, "APPROVER_REQUIRED", "Approver is required.");
        EnsureCurrentStatus(order, PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Approved);
        EnsureNotSelApproval(order, approver);

        order.Status = PurchaseOrderStatus.Approved;
        order.ReviewedBy = approver;
        order.ReviewedAt = _clock.UtcNow;
    }

    public void Reject(PurchaseOrder order, string approver, string reason)
    {
        EnsurenonNullOrWhitespace(approver, "APPROVER_REQUIRED", "Approver is required.");
        EnsurevalidRejectionReason(reason);
        EnsureCurrentStatus(order, PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Rejected);

        order.Status = PurchaseOrderStatus.Rejected;
        order.ReviewedBy = approver;
        order.ReviewedAt = _clock.UtcNow;
        order.RejectionReason = reason.Trim();
    }

    private static void EnsureCurrentStatus(PurchaseOrder order, PurchaseOrderStatus required, PurchaseOrderStatus targetForError)
    {
        if (order.Status != required)
        {
            throw new InvalidStateTransitionException(order.Status, targetForError);
        }
    }

    private static void EnsureCurrentStatusIn(PurchaseOrder order, IReadOnlyCollection<PurchaseOrderStatus> allowed, PurchaseOrderStatus targetForError)
    {
        if (!allowed.Contains(order.Status))
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

    private static void EnsurevalidRejectionReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < _minRejectionReasonLength)
        {
            throw new BusinessRuleViolationException(
                "REJECTION_REASON_INVALID",
                $"Rejection reason is required and must be at least {_minRejectionReasonLength} characters long.");
        }
    }

    private static void ClearReviewMetadata(PurchaseOrder order)
    {
        order.ReviewedBy = null;
        order.ReviewedAt = null;
        order.RejectionReason = null;
    }

}

using PoApproval.Domain.Entities;

namespace PoApproval.Domain.Services;

/// <summary>
/// Encapsulates the state-transition rules of the purchase order workflow.
/// </summary>
public interface IApprovalService
{
    /// <summary>
    /// Transitions a draft order to Submitted status.
    /// </summary>
    void Submit(PurchaseOrder order);

    /// <summary>
    /// Transitions a submitted order to Approved status.
    /// </summary>
    void Approve(PurchaseOrder order, string approver);

    /// <summary>
    /// Transitions a submitted order to Rejected status.
    /// </summary>
    void Reject(PurchaseOrder order, string approver, string reason);
}
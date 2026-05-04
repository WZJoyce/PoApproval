namespace PoApproval.Domain.Enums;

/// <summary>
/// Lifecycle states of a purchase order.
/// </summary>
public enum PurchaseOrderStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Rejected = 3
}

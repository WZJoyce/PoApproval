using System.ComponentModel.DataAnnotations;
using PoApproval.Domain.Enums;

namespace PoApproval.Api.Contracts.V1;

/// <summary>
/// Summary representation of a purchase order for list endpoints.
/// </summary>
public sealed record PurchaseOrderSummary(
    int Id,
    string OrderNo,
    decimal Amount,
    PurchaseOrderStatus Status,
    string CreatedBy,
    DateTime CreatedAt);

/// <summary>
/// Full representation of a purchase order, returned by detail endpoints.
/// </summary>
public sealed record PurchaseOrderDetails(
    int Id,
    string OrderNo,
    decimal Amount,
    PurchaseOrderStatus Status,
    string CreatedBy,
    DateTime CreatedAt,
    string? ReviewedBy,
    DateTime? ReviewedAt,
    string? RejectionReason);

/// <summary>
/// Request payload for creating a draft purchase order.
/// </summary>
public sealed record CreatePurchaseOrderRequest(
    [Required, StringLength(50, MinimumLength = 3)]
    string OrderNo,

    [Range(0.01, 1_000_000_000)]
    decimal Amount);

/// <summary>
/// Request payload for approving a submitted purchase order.
/// </summary>
public sealed record ApprovePurchaseOrderRequest(
    [Required, StringLength(100, MinimumLength = 1)]
    string Approver);

/// <summary>
/// Request payload for rejecting a submitted purchase order.
/// </summary>
public sealed record RejectPurchaseOrderRequest(
    [Required, StringLength(100, MinimumLength = 1)]
    string Approver,

    [Required, StringLength(500, MinimumLength = 10)]
    string Reason);

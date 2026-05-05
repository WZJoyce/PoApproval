using PoApproval.Domain.Enums;

namespace PoApproval.Domain.Entities;

/// <summary>
/// Represents a purchase order moving through the approval workflow.
/// </summary>
public sealed class PurchaseOrder
{
    public int Id { get; set; }

    public string OrderNo { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public PurchaseOrderStatus Status { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    /// <summary>
    /// Concurrency token managed by EF Core (mapped to SQL Server rowversion).
    /// </summary>
    public byte[]? RowVersion { get; set; }
}
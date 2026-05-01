using System.ComponentModel.DataAnnotations;

namespace PoApproval.Domain.Configuration;

/// <summary>
/// Business rules governing the purchase order approval workflow.
/// Bound from the "Approval" configuration section.
/// </summary>
public sealed class ApprovalSettings
{
    public const string SectionName = "Approval";

    /// <summary>
    /// Orders at or below this amount are auto-approved on submission.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "AutoApproveBelow must be non-negative.")]
    public decimal AutoApproveBelow { get; init; }

    /// <summary>
    /// Orders at or above this amount require a second approver.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "RequireSecondApprovalAbove must be non-negative.")]
    public decimal RequireSecondApprovalAbove { get; init; }

    /// <summary>
    /// Minimum length of a rejection reason in characters.
    /// </summary>
    [Range(1, 500)]
    public int RejectionReasonMinLength { get; init; } = 10;
    
    /// <summary>
    /// Maximum length of a rejection reason in characters.
    /// </summary>
    [Range(1, 500)]
    public int RejectionReasonMaxLength { get; init; } = 300;
}
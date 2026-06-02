namespace PoApproval.Domain.Advisory;

/// <summary>
/// Aggregated historical statistics for a requester (CreatedBy), used to provide context for advisory recommendations.
/// </summary>
public sealed record RequesterHistory
{
    public required string Requester { get; init; }
    public required int TotalOrders { get; init; }
    public required decimal AverageAmount { get; init; }
    public required decimal MaxAmount { get; init; }
    public required int RejectedCount { get; init; }

    public double RejectionRate =>
        TotalOrders == 0 ? 0 : (double)RejectedCount / TotalOrders;
}

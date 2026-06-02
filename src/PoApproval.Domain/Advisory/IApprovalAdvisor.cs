namespace PoApproval.Domain.Advisory;

using PoApproval.Domain.Entities;

/// <summary>
/// Produces advisory recommendations for purchase orders under review.
/// </summary>
public interface IApprovalAdvisor
{
    Task<AdvisorRecommendation> GetRecommendationAsync(
        PurchaseOrder order,
        RequesterHistory history,
        CancellationToken cancellationToken = default);
}

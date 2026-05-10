using PoApproval.Domain.Entities;

namespace PoApproval.Domain.Services;

/// <summary>
/// Orchestrates the creation of a new purchase order in Draft status.
/// </summary>
public interface IOrderCreationService
{
    Task<PurchaseOrder> CreateDraftAsync(
        string orderNo,
        decimal amount,
        string createdBy,
        CancellationToken cancellationToken = default);
}

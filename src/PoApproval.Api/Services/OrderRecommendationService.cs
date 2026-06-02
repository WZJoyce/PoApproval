namespace PoApproval.Api.Services;

using PoApproval.Domain.Advisory;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Repositories;

public sealed class OrderRecommendationService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IApprovalAdvisor _advisor;

    public OrderRecommendationService(
        IPurchaseOrderRepository repository,
        IApprovalAdvisor advisor)
    {
        _repository = repository;
        _advisor = advisor;
    }

    public async Task<AdvisorRecommendation> GetRecommendationAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(PurchaseOrder), orderId);

        var history = await _repository.GetRequesterHistoryAsync(
            order.CreatedBy, orderId, cancellationToken);

        return await _advisor.GetRecommendationAsync(order, history, cancellationToken);
    }
}

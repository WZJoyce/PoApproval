namespace PoApproval.Api.Services;

using System.Diagnostics;
using PoApproval.Domain.Advisory;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Repositories;

public sealed class OrderRecommendationService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IApprovalAdvisor _advisor;
    private readonly ILogger<OrderRecommendationService> _logger;

    public OrderRecommendationService(
        IPurchaseOrderRepository repository,
        IApprovalAdvisor advisor,
        ILogger<OrderRecommendationService> logger)
    {
        _repository = repository;
        _advisor = advisor;
        _logger = logger;
    }

    public async Task<AdvisorRecommendation> GetRecommendationAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(PurchaseOrder), orderId);

        var history = await _repository.GetRequesterHistoryAsync(
            order.CreatedBy, orderId, cancellationToken);

        var stopwatch = Stopwatch.StartNew();
        var recommendation = await _advisor.GetRecommendationAsync(order, history, cancellationToken);
        stopwatch.Stop();

        if (recommendation.IsAvailable)
        {
            _logger.LogInformation(
                "AI advisor completed: OrderId={OrderId}, Verdict={Verdict}, " +
                "Confidence={Confidence}, FlagCount={FlagCount}, LatencyMs={LatencyMs}",
                orderId,
                recommendation.Verdict,
                recommendation.Confidence,
                recommendation.Flags.Count,
                stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogWarning(
                "AI advisor unavailable (graceful degradation): OrderId={OrderId}, LatencyMs={LatencyMs}",
                orderId,
                stopwatch.ElapsedMilliseconds);
        }
        return recommendation;
    }
}

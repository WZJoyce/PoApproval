using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Repositories;

namespace PoApproval.Domain.Services;

public sealed class OrderCreationService : IOrderCreationService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public OrderCreationService(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<PurchaseOrder> CreateDraftAsync(
        string orderNo,
        decimal amount,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByOrderNoAsync(orderNo, cancellationToken);
        if (existing is not null)
        {
            throw new BusinessRuleViolationException(
                "ORDER_NO_DUPLICATE",
                $"An order with OrderNo '{orderNo}' already exists.");
        }

        var order = new PurchaseOrder
        {
            OrderNo = orderNo,
            Amount = amount,
            CreatedBy = createdBy,
            CreatedAt = _clock.UtcNow,
            Status = PurchaseOrderStatus.Draft
        };

        await _repository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}

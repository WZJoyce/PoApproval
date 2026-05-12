using PoApproval.Domain.Entities;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Repositories;

namespace PoApproval.Domain.Services;

public sealed class OrderTransitionService : IOrderTransitionService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IApprovalService _approvalService;
    private readonly IUnitOfWork _unitOfWork;

    public OrderTransitionService(
        IPurchaseOrderRepository repository,
        IApprovalService approvalService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _approvalService = approvalService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrder> SubmitAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await GetOrThrowAsync(id, cancellationToken);
        _approvalService.Submit(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<PurchaseOrder> ApproveAsync(int id, string approver, CancellationToken cancellationToken = default)
    {
        var order = await GetOrThrowAsync(id, cancellationToken);
        _approvalService.Approve(order, approver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<PurchaseOrder> RejectAsync(int id, string approver, string reason, CancellationToken cancellationToken = default)
    {
        var order = await GetOrThrowAsync(id, cancellationToken);
        _approvalService.Reject(order, approver, reason);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order;
    }

    private async Task<PurchaseOrder> GetOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(PurchaseOrder), id);
    }
}

using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Repositories;

namespace PoApproval.Domain.Services;

public sealed class OrderQueryService : IOrderQueryService
{
    private const int _minPageSize = 1;
    private const int _maxPageSize = 100;
    private const int _defaultPageSize = 50;

    private readonly IPurchaseOrderRepository _repository;

    public OrderQueryService(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PurchaseOrder> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {

        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(PurchaseOrder), id);
    }

    public Task<PagedResult<PurchaseOrder>> ListAsync(
        PurchaseOrderStatus? statusFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var clampedPage = Math.Max(1, page);
        var clampedPageSize = Math.Clamp(pageSize, _minPageSize, _maxPageSize);

        return _repository.ListAsync(statusFilter, clampedPage, clampedPageSize, cancellationToken);
    }
}

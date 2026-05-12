using Microsoft.EntityFrameworkCore;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Repositories;

namespace PoApproval.Infrastructure.Persistence.Repositories;

public sealed class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly AppDbContext _db;

    public PurchaseOrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
       => _db.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<PurchaseOrder?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
       => _db.PurchaseOrders.FirstOrDefaultAsync(o => o.OrderNo == orderNo, cancellationToken);

    public async Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
    {
        await _db.PurchaseOrders.AddAsync(order, cancellationToken);
    }
    public async Task<PagedResult<PurchaseOrder>> ListAsync(PurchaseOrderStatus? statusFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.PurchaseOrders.AsNoTracking();

        if (statusFilter.HasValue)
        {
            query = query.Where(o => o.Status == statusFilter.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PurchaseOrder>(items, page, pageSize, totalItems);
    }
}

using Microsoft.EntityFrameworkCore;
using PoApproval.Domain.Entities;
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
    public async Task<IReadOnlyList<PurchaseOrder>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.PurchaseOrders
            .AsNoTracking()
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

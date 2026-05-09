using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PoApproval.Domain.Entities;

namespace PoApproval.Infrastructure.Persistence.Configurations;

internal sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(po => po.Id);

        builder.Property(po => po.OrderNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(po => po.OrderNo)
            .IsUnique();

        builder.Property(po => po.Amount)
            .HasPrecision(18, 2);

        builder.Property(po => po.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(po => po.CreatedAt)
            .IsRequired();

        builder.Property(po => po.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(po => po.ReviewedBy)
            .HasMaxLength(100);

        builder.Property(po => po.ReviewedAt);

        builder.Property(po => po.RejectionReason)
            .HasMaxLength(500);

        // Configure RowVersion as a concurrency token (maps to SQL Server rowversion).
        builder.Property(po => po.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}

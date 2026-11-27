
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems", table =>
        {
            table.HasCheckConstraint(
                "CK_PurchaseOrderItems_Quantity_Positive",
                "[Quantity] > 0");

            table.HasCheckConstraint(
                "CK_PurchaseOrderItems_UnitPrice_NonNegative",
                "[UnitPrice] >= 0");

            table.HasCheckConstraint(
                "CK_PurchaseOrderItems_TaxRate_Range",
                "[TaxRate] >= 0 AND [TaxRate] <= 100");

            table.HasCheckConstraint(
                "CK_PurchaseOrderItems_Amounts_NonNegative",
                "[Subtotal] >= 0 AND [TaxAmount] >= 0 AND [TotalAmount] >= 0");

            table.HasCheckConstraint(
                "CK_PurchaseOrderItems_TotalAmount",
                "[TotalAmount] = [Subtotal] + [TaxAmount]");
        });

        builder.HasKey(item => item.Id);

        builder.Property(item => item.ProductName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(item => item.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(item => item.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.TaxRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(item => item.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.PurchaseOrderId)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.HasIndex(item => item.PurchaseOrderId);

        builder.HasIndex(item => item.ProductId);

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

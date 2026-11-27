
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders", table =>
        {
            table.HasCheckConstraint(
                "CK_PurchaseOrders_Amounts_NonNegative",
                "[Subtotal] >= 0 AND [TaxAmount] >= 0 AND [TotalAmount] >= 0");

            table.HasCheckConstraint(
                "CK_PurchaseOrders_TotalAmount",
                "[TotalAmount] = [Subtotal] + [TaxAmount]");
        });

        builder.HasKey(order => order.Id);

        builder.Property(order => order.OrderNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(order => order.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.Justification)
            .HasMaxLength(1000);

        builder.Property(order => order.ExpectedDeliveryDate);

        builder.Property(order => order.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.SupplierId)
            .IsRequired();

        builder.Property(order => order.DepartmentId)
            .IsRequired();

        builder.Property(order => order.CreatedByUserId)
            .IsRequired();

        builder.Property(order => order.AssignedToUserId)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.UpdatedAt)
            .IsRequired();

        builder.HasIndex(order => order.OrderNumber)
            .IsUnique();

        builder.HasIndex(order => order.SupplierId);

        builder.HasIndex(order => order.DepartmentId);

        builder.HasIndex(order => order.CreatedByUserId);

        builder.HasIndex(order => order.AssignedToUserId);

        builder.HasIndex(order => order.Status);

        builder.HasOne(order => order.Supplier)
            .WithMany()
            .HasForeignKey(order => order.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.Department)
            .WithMany()
            .HasForeignKey(order => order.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.CreatedByUser)
            .WithMany()
            .HasForeignKey(order => order.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.AssignedToUser)
            .WithMany()
            .HasForeignKey(order => order.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.PurchaseOrder)
            .HasForeignKey(item => item.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

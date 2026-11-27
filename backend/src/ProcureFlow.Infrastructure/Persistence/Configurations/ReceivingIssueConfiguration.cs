
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class ReceivingIssueConfiguration : IEntityTypeConfiguration<ReceivingIssue>
{
    public void Configure(EntityTypeBuilder<ReceivingIssue> builder)
    {
        builder.ToTable("ReceivingIssues", table =>
        {
            table.HasCheckConstraint(
                "CK_ReceivingIssues_Quantity",
                "[Quantity] > 0");
        });

        builder.HasKey(issue => issue.Id);

        builder.Property(issue => issue.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(issue => issue.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(issue => issue.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(issue => issue.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(issue => issue.PurchaseOrderId)
            .IsRequired();

        builder.Property(issue => issue.PurchaseOrderItemId)
            .IsRequired();

        builder.Property(issue => issue.ReportedByUserId)
            .IsRequired();

        builder.Property(issue => issue.ResolvedByUserId);

        builder.Property(issue => issue.CreatedAt)
            .IsRequired();

        builder.Property(issue => issue.ResolvedAt);

        builder.HasIndex(issue => issue.PurchaseOrderId);

        builder.HasIndex(issue => issue.PurchaseOrderItemId);

        builder.HasIndex(issue => issue.Status);

        builder.HasIndex(issue => issue.ReportedByUserId);

        builder.HasIndex(issue => issue.ResolvedByUserId);

        builder.HasOne(issue => issue.PurchaseOrder)
            .WithMany()
            .HasForeignKey(issue => issue.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(issue => issue.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(issue => new
            {
                issue.PurchaseOrderId,
                issue.PurchaseOrderItemId
            })
            .HasPrincipalKey(item => new
            {
                item.PurchaseOrderId,
                item.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(issue => issue.ReportedByUser)
            .WithMany()
            .HasForeignKey(issue => issue.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(issue => issue.ResolvedByUser)
            .WithMany()
            .HasForeignKey(issue => issue.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

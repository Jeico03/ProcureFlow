
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class ApprovalConfiguration : IEntityTypeConfiguration<Approval>
{
    public void Configure(EntityTypeBuilder<Approval> builder)
    {
        builder.ToTable("Approvals");

        builder.HasKey(approval => approval.Id);

        builder.Property(approval => approval.Decision)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(approval => approval.Comments)
            .HasMaxLength(1000);

        builder.Property(approval => approval.CreatedAt)
            .IsRequired();

        builder.HasIndex(approval => approval.PurchaseOrderId);

        builder.HasIndex(approval => approval.ApproverUserId);

        builder.HasOne(approval => approval.PurchaseOrder)
            .WithMany()
            .HasForeignKey(approval => approval.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(approval => approval.ApproverUser)
            .WithMany()
            .HasForeignKey(approval => approval.ApproverUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

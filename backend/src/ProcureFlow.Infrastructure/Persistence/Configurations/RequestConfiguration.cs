using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(request => request.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(request => request.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(request => request.ReviewComments)
            .HasMaxLength(1000);

        builder.Property(request => request.PurchaseOrderId)
            .IsRequired();

        builder.Property(request => request.RequestedByUserId)
            .IsRequired();

        builder.Property(request => request.ReviewedByUserId);

        builder.Property(request => request.CreatedAt)
            .IsRequired();

        builder.Property(request => request.ReviewedAt);

        builder.HasIndex(request => request.PurchaseOrderId);

        builder.HasIndex(request => request.RequestedByUserId);

        builder.HasIndex(request => request.ReviewedByUserId);

        builder.HasIndex(request => request.Status);

        builder.HasOne(request => request.PurchaseOrder)
            .WithMany()
            .HasForeignKey(request => request.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(request => request.RequestedByUser)
            .WithMany()
            .HasForeignKey(request => request.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(request => request.ReviewedByUser)
            .WithMany()
            .HasForeignKey(request => request.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

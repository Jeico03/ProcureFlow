
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments", table =>
        {
            table.HasCheckConstraint(
                "CK_Attachments_ExactlyOneAssociation",
                """
                (
                    [PurchaseOrderId] IS NOT NULL
                    AND [RequestId] IS NULL
                    AND [ReceivingIssueId] IS NULL
                )
                OR
                (
                    [PurchaseOrderId] IS NULL
                    AND [RequestId] IS NOT NULL
                    AND [ReceivingIssueId] IS NULL
                )
                OR
                (
                    [PurchaseOrderId] IS NULL
                    AND [RequestId] IS NULL
                    AND [ReceivingIssueId] IS NOT NULL
                )
                """);

            table.HasCheckConstraint(
                "CK_Attachments_FileSize",
                "[FileSize] > 0");
        });

        builder.HasKey(attachment => attachment.Id);

        builder.Property(attachment => attachment.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(attachment => attachment.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(attachment => attachment.ContentType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(attachment => attachment.FileSize)
            .IsRequired();

        builder.Property(attachment => attachment.PurchaseOrderId);

        builder.Property(attachment => attachment.RequestId);

        builder.Property(attachment => attachment.ReceivingIssueId);

        builder.Property(attachment => attachment.UploadedByUserId)
            .IsRequired();

        builder.Property(attachment => attachment.IsDeleted)
            .IsRequired();

        builder.Property(attachment => attachment.CreatedAt)
            .IsRequired();

        builder.Property(attachment => attachment.DeletedAt);

        builder.HasIndex(attachment => attachment.StorageKey)
            .IsUnique();

        builder.HasIndex(attachment => attachment.PurchaseOrderId);

        builder.HasIndex(attachment => attachment.RequestId);

        builder.HasIndex(attachment => attachment.ReceivingIssueId);

        builder.HasIndex(attachment => attachment.UploadedByUserId);

        builder.HasIndex(attachment => attachment.IsDeleted);

        builder.HasOne(attachment => attachment.PurchaseOrder)
            .WithMany()
            .HasForeignKey(attachment => attachment.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(attachment => attachment.Request)
            .WithMany()
            .HasForeignKey(attachment => attachment.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(attachment => attachment.ReceivingIssue)
            .WithMany()
            .HasForeignKey(attachment => attachment.ReceivingIssueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(attachment => attachment.UploadedByUser)
            .WithMany()
            .HasForeignKey(attachment => attachment.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

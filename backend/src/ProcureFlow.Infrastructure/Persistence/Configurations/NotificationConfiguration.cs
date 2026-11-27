using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", table =>
        {
            table.HasCheckConstraint(
                "CK_Notifications_RelatedEntity",
                """
                (
                    [RelatedEntityType] IS NULL
                    AND [RelatedEntityId] IS NULL
                )
                OR
                (
                    [RelatedEntityType] IS NOT NULL
                    AND [RelatedEntityId] IS NOT NULL
                    AND [RelatedEntityId] > 0
                )
                """);
        });

        builder.HasKey(notification => notification.Id);

        builder.Property(notification => notification.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(notification => notification.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(notification => notification.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(notification => notification.RelatedEntityId);

        builder.Property(notification => notification.UserId)
            .IsRequired();

        builder.Property(notification => notification.IsRead)
            .IsRequired();

        builder.Property(notification => notification.ReadAt);

        builder.Property(notification => notification.IsDeleted)
            .IsRequired();

        builder.Property(notification => notification.CreatedAt)
            .IsRequired();

        builder.Property(notification => notification.DeletedAt);

        builder.HasIndex(notification => notification.UserId);

        builder.HasIndex(notification => new
        {
            notification.UserId,
            notification.IsRead,
            notification.IsDeleted
        });

        builder.HasIndex(notification => notification.CreatedAt);

        builder.HasOne(notification => notification.User)
            .WithMany()
            .HasForeignKey(notification => notification.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

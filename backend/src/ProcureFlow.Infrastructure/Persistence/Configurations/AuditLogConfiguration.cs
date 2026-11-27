using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(log => log.EntityId)
            .IsRequired();

        builder.Property(log => log.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(log => log.Details)
            .HasMaxLength(1000);

        builder.Property(log => log.UserId)
            .IsRequired();

        builder.Property(log => log.CreatedAt)
            .IsRequired();

        builder.HasIndex(log => log.UserId);

        builder.HasIndex(log => new
        {
            log.EntityType,
            log.EntityId
        });

        builder.HasIndex(log => log.CreatedAt);

        builder.HasOne(log => log.User)
            .WithMany()
            .HasForeignKey(log => log.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

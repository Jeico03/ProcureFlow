using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.ToTable("TaxConfigurations", table =>
        {
            table.HasCheckConstraint(
                "CK_TaxConfigurations_TaxRate",
                "[TaxRate] >= 0 AND [TaxRate] <= 100");
        });

        builder.HasKey(tax => tax.Id);

        builder.Property(tax => tax.TaxRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(tax => tax.IsActive)
            .IsRequired();

        builder.Property(tax => tax.Description)
            .HasMaxLength(500);

        builder.Property(tax => tax.CreatedByUserId)
            .IsRequired();

        builder.Property(tax => tax.CreatedAt)
            .IsRequired();

        builder.HasIndex(tax => tax.IsActive)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasIndex(tax => tax.CreatedByUserId);

        builder.HasOne(tax => tax.CreatedByUser)
            .WithMany()
            .HasForeignKey(tax => tax.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

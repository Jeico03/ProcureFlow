
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(supplier => supplier.TaxId)
            .HasMaxLength(50);

        builder.Property(supplier => supplier.ContactName)
            .HasMaxLength(150);

        builder.Property(supplier => supplier.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(supplier => supplier.Phone)
            .HasMaxLength(30);

        builder.Property(supplier => supplier.Address)
            .HasMaxLength(300);

        builder.Property(supplier => supplier.IsActive)
            .IsRequired();

        builder.Property(supplier => supplier.CreatedAt)
            .IsRequired();

        builder.Property(supplier => supplier.UpdatedAt)
            .IsRequired();

        builder.HasIndex(supplier => supplier.Email)
            .IsUnique();
    }
}

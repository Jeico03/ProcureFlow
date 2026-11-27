
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(department => department.Id);

        builder.Property(department => department.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(department => department.Description)
            .HasMaxLength(250);

        builder.Property(department => department.IsActive)
            .IsRequired();

        builder.Property(department => department.CreatedAt)
            .IsRequired();

        builder.Property(department => department.UpdatedAt)
            .IsRequired();

        builder.HasIndex(department => department.Name)
            .IsUnique();
    }
}

using Employees.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employees.Api.Context.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Contacts)
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        // Если в Entity есть CreatedAt/UpdatedAt, их тоже стоит настроить
        // builder.Property(e => e.CreatedAt).IsRequired();
        // builder.Property(e => e.UpdatedAt);
    }
}
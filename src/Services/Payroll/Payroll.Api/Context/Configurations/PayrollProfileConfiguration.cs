using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payroll.Api.Domain;

namespace Payroll.Api.Context.Configurations;

public class PayrollProfileConfiguration : IEntityTypeConfiguration<PayrollProfile>
{
    public void Configure(EntityTypeBuilder<PayrollProfile> builder)
    {
        builder.ToTable("payroll-profiles");

        builder.HasKey(pp => pp.Id);
        
        builder.HasIndex(pp => pp.EmployeeId)
               .IsUnique();

        builder.Property(pp => pp.BaseSalary)
               .HasPrecision(18, 2); 

        builder.Property(pp => pp.Currency)
               .HasMaxLength(3)
               .IsRequired();
    }
}

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Payroll.Api.Domain;

namespace Payroll.Api.Context;

public class PayrollDbContext(DbContextOptions<PayrollDbContext> options) 
    : DbContext(options)
{
    public DbSet<PayrollProfile> PayrollProfiles => Set<PayrollProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // за раз применяем все конфигурации сборки
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}

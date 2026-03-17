using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Payroll.Api.Context;

namespace Payroll.Api;

// фабрика нужна для сборки сомого контекста бд, т.к. без запущеного хоста Aspire он не соберется
public class PayrollDbContextFactory: IDesignTimeDbContextFactory<PayrollDbContext>
{
    public PayrollDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PayrollDbContext>();

        optionsBuilder.UseNpgsql(); 

        return new PayrollDbContext(optionsBuilder.Options);
    }
}

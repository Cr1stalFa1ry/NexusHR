using Employees.Api.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Employees.Api;

// создание экземпляра контекста бд без строк подключения, чтобы не выкидывало ошибку при создании новой миграции
public class EmployeesDbContextFactory : IDesignTimeDbContextFactory<EmployeesDbContext>
{
    public EmployeesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EmployeesDbContext>();

        optionsBuilder.UseNpgsql(); 

        return new EmployeesDbContext(optionsBuilder.Options);
    }
}
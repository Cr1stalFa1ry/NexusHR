using System.Reflection;
using Employees.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Employees.Api.Context;

public class EmployeesDbContext(DbContextOptions<EmployeesDbContext> options) :
    DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // за раз применяем все конфигурации сборки
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // This enables your DbContext to map the incoming and
        // outgoing messages as part of the outbox
        //modelBuilder.MapWolverineEnvelopeStorage("wolverine"); //? Без этого работает и обработчик и метод контроллера
    }
}

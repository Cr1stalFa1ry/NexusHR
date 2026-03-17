using System;
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
        // за раз применяем все конфигурации сборки
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // добавление таблиц для outbox и inbox сообщений
        modelBuilder.MapWolverineEnvelopeStorage();

        base.OnModelCreating(modelBuilder);
    }
}

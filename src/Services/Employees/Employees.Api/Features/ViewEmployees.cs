using Employees.Api.Context;
using Employees.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Attributes;

namespace Employees.Api.Features.Viewing;

public record GetEmployeesCommand(int Page, int PageSize);
public record GetEmployeeByIdCommand(Guid Id);

[ApiController]
[Route("/api/employees")]
public class ViewEmployeesController : ControllerBase
{
    [HttpGet]
    public async Task<IResult> Get(
        [FromQuery] GetEmployeesCommand command, 
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var employees = await bus.InvokeAsync<List<Employee>>(command, cancellationToken);
        return Results.Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetById(
        [FromRoute] Guid id, 
        [FromServices] IMessageBus bus, 
        CancellationToken cancellationToken)
    {
        var employee = await bus.InvokeAsync<Employee>(new GetEmployeeByIdCommand(id), cancellationToken);

        if (employee == null)
            return Results.NotFound($"Employee with id: {id} does not exists.");

        return Results.Ok(employee);
    }
}

public class ViewEmployeeHandler
{
    /// <summary>
    /// Get sequence of Employees
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    [NonTransactional]
    public static async Task<List<Employee>> Handle(
        GetEmployeesCommand command,
        CancellationToken cancellationToken, 
        EmployeesDbContext dbContext)
    {
        int page = Math.Max(1, command.Page);
        int pageSize = Math.Clamp(command.PageSize, 1, 100);

        var employees = await dbContext.Employees
            .AsNoTracking()
            .OrderBy(emp => emp.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return employees;
    }

    /// <summary>
    /// Get employee by Id
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    [NonTransactional]
    public static async Task<Employee?> Handle(
        GetEmployeeByIdCommand command,
        CancellationToken cancellationToken, 
        EmployeesDbContext dbContext)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(emp => emp.Id == command.Id, cancellationToken);

        return employee;
    }
}
using System;
using Employees.Api.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Attributes;

namespace Employees.Api.Features;

public record FireEmployeeCommand(Guid Id);

[ApiController]
[Route("/api/employees")]
public class FireEmployeesController : ControllerBase
{
    [HttpDelete("{id:guid}")]
    public async Task<IResult> Fire(
        [FromRoute] Guid id, 
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<bool>(new FireEmployeeCommand(id), cancellationToken);
        return result ? Results.NoContent() : Results.NotFound($"Employee {id} not found");
    }
}

public class FireEmployeeHandler(EmployeesDbContext dbContext)
{
    [NonTransactional]
    public async Task<bool> Handle(
        FireEmployeeCommand command,
        CancellationToken ct)
    {
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(emp => emp.Id == command.Id, ct);

        if (employee == null) return false;

        employee.Fire();

        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}

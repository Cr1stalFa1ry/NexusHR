using Employees.Api.Context;
using Employees.Api.Domain;
using Wolverine;
using Employees.Contracts.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.EntityFrameworkCore;

namespace Employees.Api.Features.Hiring;

public record HireEmployeeCommand(string FirstName, string LastName, string Email, string? Contacts);

[ApiController]
[Route("/api")]
public class EmployeesController : ControllerBase
{
    [HttpPost("/employees")]
    public async Task<IResult> Post([FromBody] HireEmployeeCommand command, [FromServices] IMessageBus bus)
    {
        var hiredEmployee = await bus.InvokeAsync<Employee>(command);
        return Results.Ok(new { employeeId = hiredEmployee.Id });
    }

    [HttpPost("/employees-test-with-db_context_outbox")]
    public async Task Post2(
        [FromBody] HireEmployeeCommand command,
        [FromServices] IDbContextOutbox<EmployeesDbContext> outbox)
    {
        var employee = new Employee(
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.Contacts
        );

        // Add the item to the current
        // DbContext unit of work
        outbox.DbContext.Add(employee);

         // Publish a message to take action on the new item
        // in a background thread
        await outbox.PublishAsync(new HiredEmployee(
            employee.Id, 
            employee.FirstName, 
            employee.LastName, 
            employee.Email, 
            employee.Contacts
        ));

        // Commit all changes and flush persisted messages
        // to the persistent outbox
        // in the correct order
        await outbox.SaveChangesAndFlushMessagesAsync();
    }
}
 
public static class HireEmployeeHandler
{
    [Transactional]
    public static (Employee, HiredEmployee) Handle(
        HireEmployeeCommand command, 
        EmployeesDbContext dbContext)
    {
        var employee = new Employee(
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.Contacts
        );

        // сохранение в бд нового работника
        dbContext.Employees.Add(employee);

        // отправка каскаодного сообщения
        // This event being returned
        // by the handler will be automatically sent
        // out as a "cascading" message
        return (employee, new HiredEmployee(
            employee.Id, 
            employee.FirstName, 
            employee.LastName, 
            employee.Email, 
            employee.Contacts
        ));
    }
}


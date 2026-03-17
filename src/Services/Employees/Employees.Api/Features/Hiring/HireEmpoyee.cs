using Employees.Api.Context;
using Employees.Api.Domain;
using Wolverine;
using Employees.Contracts.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine.EntityFrameworkCore;

namespace Employees.Api.Features.Hiring;

// контракт
public record HireEmployeeCommand(string FirstName, string LastName, string Email, string? Contacts);

// minimal api
// public static class EmployeeEndpoints
// {
//     public static IEndpointRouteBuilder MapEmployees(this IEndpointRouteBuilder app)
//     {
//         app.MapPost("/api/employees", async (HireEmployeeCommand command, IMessageBus bus) =>
//         {
//             // прокидываем как в MediatR запрос и получаем ответ, варинат работы Request/Response
//             var result = await bus.InvokeAsync<HiredEmployee>(command);
//             // если бы я выкинул комманду без возвращения результата:
//             // await bus.InvokeAsync(command); то событие бы отправилось в очередь брокера сообщений

//             // публикуем сообщение в очередь
//             await bus.PublishAsync(result);

//             return Results.Created($"/api/employees/{result.EmployeeId}", result);
//         });

//         return app;
//     }
// }

[ApiController]
[Route("/api")]
public class EmployeesController : ControllerBase
{
    [HttpPost("/employees")]
    public async Task Post(
        [FromBody] HireEmployeeCommand command,
        [FromServices] IDbContextOutbox<EmployeesDbContext> outbox
    )
    {
        // create new employee
        var employee = new Employee(
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.Contacts
        );

        // add employee to the current DbContext unit of work
        outbox.DbContext.Employees.Add(employee);

        // publish message to take action on the new employee in a background thread
        await outbox.PublishAsync(new HiredEmployee(
                employee.Id, 
                employee.FirstName, 
                employee.LastName, 
                employee.Email, 
                employee.Contacts
            )
        );

        
        // Commit all changes and flush persisted messages
        // to the persistent outbox
        // in the correct order
        await outbox.SaveChangesAndFlushMessagesAsync();
    }
}

// обработчик 
// public static class HireEmployeeHandler
// {
//     public async static Task<HiredEmployee> Handle(
//         HireEmployeeCommand command, 
//         EmployeesDbContext dbContext)
//     {
//         var employee = new Employee(
//             command.FirstName, 
//             command.LastName, 
//             command.Email, 
//             command.Contacts
//         );

//         // сохранение в бд нового работника
//         dbContext.Employees.Add(employee);
//         await dbContext.SaveChangesAsync();

//         // отправка сообщения в другие микросервисы
//         return new HiredEmployee(
//             employee.Id, 
//             employee.FirstName, 
//             employee.LastName, 
//             employee.Email, 
//             employee.Contacts
//         );
//     }
// }


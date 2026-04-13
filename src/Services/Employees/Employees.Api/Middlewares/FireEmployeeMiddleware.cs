using System;
using Employees.Api.Context;
using Employees.Api.Features;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Employees.Api.Middlewares;

public static class FireEmployeeMiddleware
{
    public static async Task<(HandlerContinuation, IResult)> Before(
        FireEmployeeCommand command, 
        EmployeesDbContext dbContext, 
        CancellationToken ct)
    {
        var exists = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (exists == null)
        {
            return (HandlerContinuation.Stop, Results.NotFound($"Employee {command.Id} not found"));
        }

        return (HandlerContinuation.Continue, Results.Empty);
    }
}
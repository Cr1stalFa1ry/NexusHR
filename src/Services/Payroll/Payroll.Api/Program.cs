using Wolverine;
using Wolverine.RabbitMQ;
using Payroll.Api.Context;
using Microsoft.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using JasperFx.Core;
using Employees.Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWolverine(opts =>
{
    // получаем строку подключения по messging, которую настроили в AppHost
    var connectionString = builder.Configuration.GetConnectionString("messaging");

    opts.UseRabbitMq(new Uri(connectionString!))
        .AutoProvision();

    opts.ListenToRabbitQueue("hired-employees");

    opts.PublishMessage<BonusAwarded>().ToRabbitQueue("notifications");

    // если произойдет ошибка при создании профиля, или любой другой сущности, то Wolverine трижды попытается 
    // создать сообщения с нарастающей паузой.
    opts.Policies.OnException<Exception>().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
});

builder.AddNpgsqlDbContext<PayrollDbContext>("payroll-db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigrations(app);
}

app.MapDefaultEndpoints();

app.MapGet("/api/payroll/{employeeId:guid}", async (Guid employeeId, PayrollDbContext dbContext) =>
{
    var profile = await dbContext.PayrollProfiles
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.EmployeeId == employeeId);

    return profile is not null 
        ? Results.Ok(profile) 
        : Results.NotFound();
});

app.Run();

static async Task ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PayrollDbContext>();

    for (int i = 1; i <= 10; i++)
    {
        try
        {
            await context.Database.MigrateAsync();
            Console.WriteLine("Database migrated successfully.");
            break;
        }
        catch (Exception)
        {
            Console.WriteLine($"Migration attempt {i} failed. Waiting...");
            if (i == 5) throw;
            await Task.Delay(5000); 
        }
    }
}
using Employees.Api.Context;
using Employees.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Wolverine.EntityFrameworkCore;
using JasperFx.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// register context to DI, employee-db indicated in AppHost
// также влючает pooling, retries, corresponding health check, logging and telemetry
builder.AddNpgsqlDbContext<EmployeesDbContext>("employee-db", opt =>
{
    opt.DisableRetry = true;
}); 
// была проблема в обработчике при использовании Transactional Middleware, конфликтовали две стратегии: со стороны 
// бд и со стороны самого Wolverine (именно transactional middleware) - opt.DisableRetry = true

builder.Host.UseWolverine(opts =>
{
    // явное сканирование сборки, где лежит текущий проект, для нахождения всех классов, 
    // у которых есть суффикс Handler
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // get connection strings from messaging and employee-db, which configure in Apphost
    var connectionStringRabbitMQ = builder.Configuration.GetConnectionString("messaging");
    var connectionStringPostgres = builder.Configuration.GetConnectionString("employee-db");


    // setting up storage messages (outbox/inbox)
    opts.PersistMessagesWithPostgresql(connectionStringPostgres!, "wolverine");

    // enable outbox for Ef Core and register necessary services, including IDbContextOutbox<T>
    // Set up Entity Framework Core as the support
    // for Wolverine's transactional middleware
    opts.UseEntityFrameworkCoreTransactions();

    // opts.Policies.UseDurableLocalQueues();

    // создание 
    opts.Services.AddResourceSetupOnStartup();

    opts.UseRabbitMq(new Uri(connectionStringRabbitMQ!))
        .AutoProvision();

    //opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

    opts.PublishMessage<HiredEmployee>()
        .ToRabbitQueue("hired-employees")
        .UseDurableOutbox();
});

var app = builder.Build();

// автоматическая миграция
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ApplyMigrations(app);
}

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();

// при запуске приложения, Employees.Api собирается быстрее чем контейнер postgres, поэтому ждем пока он не соберется
static async Task ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EmployeesDbContext>();

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

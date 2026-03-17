using Employees.Api.Context;
using Employees.Api.Features.Hiring;
using Employees.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Wolverine.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.AddNpgsqlDbContext<EmployeesDbContext>("employee-db"); // регистрируем контекст в di, database указано в AppHost

builder.Host.UseWolverine(opts =>
{
    // Указываем Wolverine использовать EF Core для Outbox
    // Это автоматически зарегистрирует IDbContextOutbox в DI
    opts.UseEntityFrameworkCoreOutbox<EmployeesDbContext>();

    // явное сканирование сборки, где лежит текущий проект, для нахождения всех классов, 
    // у которых есть суффикс Handler
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // получаем строки подключения по messging и employee-db, которые настроили в AppHost
    var connectionStringRabbitMQ = builder.Configuration.GetConnectionString("messaging");
    var connectionStringPostgres = builder.Configuration.GetConnectionString("employee-db");

    // настройка хранилищ сообщений (outbox/inbox)
    opts.PersistMessagesWithPostgresql(connectionStringPostgres!);

    opts.UseRabbitMq(new Uri(connectionStringRabbitMQ!))
        .AutoProvision();

    opts.PublishMessage<HiredEmployee>().ToRabbitQueue("hired-employees");
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

// вызов метода расширения с minimal api 
//app.MapEmployees();
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

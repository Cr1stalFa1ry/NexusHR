using JasperFx.Core;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Host.UseWolverine(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("messaging");
    opts.UseRabbitMq(new Uri(connectionString!))
        .AutoProvision();

     opts.ListenToRabbitQueue("notifications");

     opts.Policies.OnException<Exception>().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();


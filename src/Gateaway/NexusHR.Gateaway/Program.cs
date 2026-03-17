
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // стандартные настройки Aspire (логи, метрики и т.д.)

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "NexusHR Gateway", Version = "v1" });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/api/employees", "Swagger Employees Service");
    });
}
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/api/employees/swagger/v1/swagger.json", "Employees Service");
//     options.SwaggerEndpoint("/api/payroll/swagger/v1/swagger.json", "Payroll Service");
// });

// Включаем стандартные эндпоинты Aspire (Health Checks и т.д.)
app.UseHttpsRedirection();

app.MapReverseProxy();

app.MapDefaultEndpoints();

app.Run();

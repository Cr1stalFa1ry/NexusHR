
var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin(); // доступ к UI RabbitMQ (очереди, обмены)
    //.WithDataVolume(); // очереди и сообщения не удаляются при перезагрузке AppHost

// var postgres = builder.AddPostgres("postgres")
//     .WithPgAdmin();
    //WithDataVolume(); // чтобы данные не пропадали

var emploeyy_db = builder.AddConnectionString("employee-db");
var payroll_db = builder.AddConnectionString("payroll-db");

var employees = builder.AddProject<Projects.Employees_Api>("employees-api")
    .WithReference(emploeyy_db)
    .WithReference(rabbit);

var payroll = builder.AddProject<Projects.Payroll_Api>("payroll-api")
    .WithReference(payroll_db)
    .WithReference(rabbit);

var notifications = builder.AddProject<Projects.Notifications_Api>("notifications-api")
    .WithReference(rabbit);

// добавляем gateaway и ссылки на сервисы
var gateaway = builder
    .AddProject<Projects.NexusHR_Gateaway>("gateaway")
    .WithReference(employees)
    .WithReference(payroll)   
    .WithExternalHttpEndpoints();

builder.Build().Run();

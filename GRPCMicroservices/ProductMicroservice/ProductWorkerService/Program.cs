using ProductWorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<ProductFactory>();

var host = builder.Build();

host.Run();
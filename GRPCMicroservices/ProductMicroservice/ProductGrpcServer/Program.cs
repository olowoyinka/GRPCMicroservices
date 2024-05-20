using Microsoft.EntityFrameworkCore;
using ProductGrpcServer.Data;
using ProductGrpcServer.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;
});

builder.Services.AddDbContext<ProductsContext>(options =>
                            options.UseInMemoryDatabase("Products"));

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.MigrateDatabase();

app.MapGrpcService<ProductService>();

app.Run();



public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication webApp)
    {
        using (var scope = webApp.Services.CreateScope())
        {
            using (var appContext = scope.ServiceProvider.GetRequiredService<ProductsContext>())
            {
                try
                {
                    ProductsContextSeed.SeedAsync(appContext);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        return webApp;
    }
}
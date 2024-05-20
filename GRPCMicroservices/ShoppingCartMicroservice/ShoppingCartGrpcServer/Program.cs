using DiscountGrpcServer.Protos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartGrpcServer.Data;
using ShoppingCartGrpcServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShoppingCartContext>(options =>
                                                    options.UseInMemoryDatabase("ShoppingCart"));

builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (o => o.Address = new Uri(builder.Configuration["GrpcConfigs:DiscountUrl"]));

builder.Services.AddScoped<DiscountService>();

builder.Services.AddAuthentication("Bearer")
 .AddJwtBearer("Bearer", options =>
 {
     options.Authority = "https://localhost:7288";
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateAudience = false
     };
 });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MigrateDatabase();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ShoppingCartService>();

app.Run();


public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication webApp)
    {
        using (var scope = webApp.Services.CreateScope())
        {
            using (var appContext = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>())
            {
                try
                {
                    ShoppingCartContextSeed.SeedAsync(appContext);
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
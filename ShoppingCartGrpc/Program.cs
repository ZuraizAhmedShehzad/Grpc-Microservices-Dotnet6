using DiscountGrpc.Protos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (o => o.Address = new Uri(builder.Configuration.GetValue<string>("GrpcConfigs:DiscountUrl")));
builder.Services.AddScoped<DiscountService>();
builder.Services.AddDbContext<ShoppingCartContext>(options =>
{
    options.UseInMemoryDatabase("ShoppingCart");
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
     {
         options.Authority = "https://localhost:7039";
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateAudience = false
         };
     });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<ShoppingCartService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
SeedDatabase(app);
app.Run();


static void SeedDatabase(IHost host)
{
    using var scope = host.Services.CreateScope();
    var service = scope.ServiceProvider;
    var productContext = service.GetRequiredService<ShoppingCartContext>();
    ShoppingCartContextSeed.SeedAsync(productContext);
}

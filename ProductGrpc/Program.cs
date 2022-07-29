using Microsoft.EntityFrameworkCore;
using ProductGrpc.Data;
using ProductGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;
});
builder.Services.AddDbContext<ProductsContext>(options =>
{
    options.UseInMemoryDatabase("Products");
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<ProductService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
SeedDatabase(app);
app.Run();


static void SeedDatabase(IHost host)
{
    using var scope = host.Services.CreateScope();
    var service = scope.ServiceProvider;
    var productContext = service.GetRequiredService<ProductsContext>();
    ProductsContextSeed.SeedAsync(productContext);
}

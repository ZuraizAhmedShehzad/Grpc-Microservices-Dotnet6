using IdentityServerService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
            .AddInMemoryClients(Config.Client)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();

app.MapGet("/", () => "Hello World!");

app.Run();

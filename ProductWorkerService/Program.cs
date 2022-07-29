using ProductWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<ProductFactory>();
    })
    .Build();

await host.RunAsync();

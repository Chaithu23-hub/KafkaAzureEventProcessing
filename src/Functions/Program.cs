using Application.Orders;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        var ordersApiBaseUrl = ctx.Configuration["OrdersApiBaseUrl"]
            ?? ctx.Configuration["Values:OrdersApiBaseUrl"]
            ?? "http://localhost:5050";

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ProcessOrderEventCommand).Assembly));

        services.AddInfrastructure(ordersApiBaseUrl);
    })
    .Build();

host.Run();

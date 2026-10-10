using Application.Common;
using Infrastructure.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string ordersApiBaseUrl)
    {
        services
            .AddHttpClient<IOrderApiClient, OrderApiClient>(client =>
            {
                client.BaseAddress = new Uri(ordersApiBaseUrl);
            })
            .AddStandardResilienceHandler(); // automatic retries / circuit-breaker via Microsoft.Extensions.Http.Resilience

        return services;
    }
}

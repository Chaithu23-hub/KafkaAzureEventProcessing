using System.Net.Http.Json;
using Application.Common;
using Application.Orders;

namespace Infrastructure.Http;

// Typed HttpClient that POSTs an order event to the Orders API
public class OrderApiClient : IOrderApiClient
{
    private readonly HttpClient _http;

    public OrderApiClient(HttpClient http) => _http = http;

    public async Task CreateOrderAsync(OrderEventDto order, CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync("/api/orders", order, cancellationToken);
        response.EnsureSuccessStatusCode(); // throws on non-2xx so the Function retries
    }
}

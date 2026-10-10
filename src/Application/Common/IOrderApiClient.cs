namespace Application.Common;

using Application.Orders;

public interface IOrderApiClient
{
    Task CreateOrderAsync(OrderEventDto order, CancellationToken cancellationToken = default);
}

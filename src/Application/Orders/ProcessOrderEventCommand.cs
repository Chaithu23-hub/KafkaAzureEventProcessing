using Application.Common;
using MediatR;

namespace Application.Orders;

public record ProcessOrderEventCommand(OrderEventDto Order) : IRequest;

public class ProcessOrderEventHandler : IRequestHandler<ProcessOrderEventCommand>
{
    private readonly IOrderApiClient _orderApiClient;

    public ProcessOrderEventHandler(IOrderApiClient orderApiClient)
    {
        _orderApiClient = orderApiClient;
    }

    public async Task Handle(ProcessOrderEventCommand request, CancellationToken cancellationToken)
    {
        // Throws on failure so Kafka retries the message
        await _orderApiClient.CreateOrderAsync(request.Order, cancellationToken);
    }
}

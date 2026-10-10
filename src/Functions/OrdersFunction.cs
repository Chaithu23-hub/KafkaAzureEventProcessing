using Application.Orders;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Functions;

public class OrdersFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersFunction> _logger;

    public OrdersFunction(IMediator mediator, ILogger<OrdersFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Kafka trigger – deserializes the raw string value and dispatches the MediatR command.
    // Throws on any failure so Kafka retries via the configured consumer-group offsets.
    [Function("OrdersConsumer")]
    public async Task Run(
        [KafkaTrigger("BrokerList", "orders",
            ConsumerGroup = "orders-fn-group",
            Protocol = BrokerProtocol.NotSet,
            AuthenticationMode = BrokerAuthenticationMode.NotSet)]
        string message,
        FunctionContext context)
    {
        _logger.LogInformation("Received Kafka message: {Message}", message);

        string valueJson = message;
        try
        {
            using var doc = JsonDocument.Parse(message);
            if (doc.RootElement.TryGetProperty("Value", out var valProp) && valProp.ValueKind == JsonValueKind.String)
            {
                valueJson = valProp.GetString() ?? message;
            }
        }
        catch
        {
            // Not wrapped, use raw message
        }

        var dto = JsonSerializer.Deserialize<OrderEventDto>(valueJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Cannot deserialize OrderEventDto.");

        await _mediator.Send(new ProcessOrderEventCommand(dto));
    }
}

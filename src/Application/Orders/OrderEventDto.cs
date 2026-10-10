namespace Application.Orders;

public record OrderEventDto(string OrderId, string CustomerName, decimal Amount, DateTime CreatedAtUtc);

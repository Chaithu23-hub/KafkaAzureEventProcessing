namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

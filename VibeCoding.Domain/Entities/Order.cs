using System.Text.Json.Serialization;

namespace VibeCoding.Domain.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus { Pending, Approved, Rejected }

public record Order(Guid Id, Guid CustomerId, decimal Amount, OrderStatus Status = OrderStatus.Pending)
{
    public Order Approve() => this with { Status = OrderStatus.Approved };
    public Order Reject() => this with { Status = OrderStatus.Rejected };
}

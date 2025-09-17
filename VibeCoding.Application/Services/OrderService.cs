using VibeCoding.Domain.Entities;
using VibeCoding.Domain.Repositories;
using VibeCoding.Domain.Shared;


namespace VibeCoding.Application.Services;

public class OrderService(IOrderRepository repository)
{
    public async Task<Result<Order>> ApproveAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id);
        if (order is null) return Result<Order>.Failure("Order not found");

        var updated = order.Approve();
        await repository.SaveAsync(updated);

        return Result<Order>.Success(updated);
    }
}
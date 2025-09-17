using VibeCoding.Domain.Entities;

namespace VibeCoding.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task SaveAsync(Order order);
}
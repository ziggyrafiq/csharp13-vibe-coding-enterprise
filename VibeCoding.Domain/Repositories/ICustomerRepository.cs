using VibeCoding.Domain.Entities;

namespace VibeCoding.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task SaveAsync(Customer customer);
}

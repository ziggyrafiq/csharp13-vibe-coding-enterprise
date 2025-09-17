using System.Text.Json;
using VibeCoding.Domain.Entities;
using VibeCoding.Domain.Repositories;

namespace VibeCoding.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();
    private readonly string _dataFilePath;

    public InMemoryCustomerRepository()
    {
        
        var basePath = AppContext.BaseDirectory;
        var jsonPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\.."));
        _dataFilePath = Path.Combine(jsonPath, "VibeCoding.Data", "customers.json");

        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            var customers = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (customers is not null)
            {
                foreach (var c in customers)
                    _customers[c.Id] = c;
            }
        }
    }

    public Task<Customer?> GetByIdAsync(Guid id) =>
        Task.FromResult(_customers.TryGetValue(id, out var customer) ? customer : null);

    public Task SaveAsync(Customer customer)
    {
        _customers[customer.Id] = customer;

        var json = JsonSerializer.Serialize(_customers.Values, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);

        return Task.CompletedTask;
    }
}
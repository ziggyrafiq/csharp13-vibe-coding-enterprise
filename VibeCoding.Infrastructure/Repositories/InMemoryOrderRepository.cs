using System.Text.Json;
using VibeCoding.Domain.Entities;
using VibeCoding.Domain.Repositories;

namespace Vibe.Infrastructure.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();
    private readonly string _dataFilePath;

    public InMemoryOrderRepository()
    {
        var basePath = AppContext.BaseDirectory;
        var jsonPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\.."));
        _dataFilePath = Path.Combine(jsonPath, "VibeCoding.Data", "orders.json");

        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            var orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            });

            if (orders is not null)
            {
                foreach (var order in orders)
                    _orders[order.Id] = order;
            }
        }
    }

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_orders.TryGetValue(id, out var order) ? order : null);

    public Task SaveAsync(Order order)
    {
        _orders[order.Id] = order;

        var json = JsonSerializer.Serialize(_orders.Values, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);

        return Task.CompletedTask;
    }
}

using System.Net.Http.Json;
using VibeCoding.Domain.Entities;
using VibeCoding.Domain.Repositories;

namespace Vibe.Application.Services;

public class CustomerService(HttpClient httpClient, ICustomerRepository repository)
{
    private static readonly string[] _allowedRoles = ["Admin", "User", "Auditor"];
    public async Task<Customer?> GetCustomerAsync(Guid id, string role)
    {
        if (!_allowedRoles.Contains(role))
            throw new UnauthorizedAccessException($"Role {role} not allowed.");

        return await repository.GetByIdAsync(id)
            ?? await httpClient.GetFromJsonAsync<Customer>($"customers/{id}");
    }


}

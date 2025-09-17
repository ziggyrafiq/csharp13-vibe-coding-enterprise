using Vibe.Application.Services;
using Vibe.Infrastructure.Repositories;
using VibeCoding.Application.Services;
using VibeCoding.Domain.Repositories;
using VibeCoding.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddScoped<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddHttpClient<CustomerService>(client =>
{
    var baseUrl = builder.Configuration["VibeCodingApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!); 
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/api/customers/{id:guid}",
    async (Guid id, string role, CustomerService service, ILogger<CustomerService> logger) =>
    {
        try
        {
            var customer = await service.GetCustomerAsync(id, role);
            return customer is not null ? Results.Ok(customer) : Results.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unauthorized access for customer {CustomerId} with role {Role}", id, role);
            return Results.Forbid();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching customer {CustomerId}", id);
            return Results.Problem("An unexpected error occurred.");
        }
    });

app.MapPost("/api/orders/{id:guid}/approve",
    async (Guid id, OrderService service, ILogger<OrderService> logger) =>
    {
        try
        {
            var result = await service.ApproveAsync(id);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error approving order {OrderId}", id);
            return Results.Problem("An unexpected error occurred.");
        }
    });

app.Run();
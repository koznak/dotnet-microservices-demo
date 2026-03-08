using System.Collections.Concurrent;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var catalog = new Dictionary<int, CatalogItem>
{
    [1] = new(1, "SKU-COFFEE-001", "Colombian Coffee Beans", 18.90m, "USD"),
    [2] = new(2, "SKU-MUG-002", "Ceramic Mug", 11.50m, "USD"),
    [3] = new(3, "SKU-FRENCHPRESS-003", "French Press", 34.00m, "USD")
};

var orders = new ConcurrentDictionary<Guid, OrderSummary>();

app.MapGet("/health", () => Results.Ok(new { service = "orders", status = "healthy", timestampUtc = DateTime.UtcNow }));

app.MapGet("/api/orders", () => Results.Ok(orders.Values.OrderByDescending(x => x.CreatedUtc)));

app.MapGet("/api/orders/{id:guid}", (Guid id) =>
{
    return orders.TryGetValue(id, out var order)
        ? Results.Ok(order)
        : Results.NotFound(new { message = $"Order '{id}' was not found." });
});

app.MapPost("/api/orders", (CreateOrderRequest request) =>
{
    try
    {
        var order = PricingEngine.BuildOrder(request, catalog);
        orders.TryAdd(order.OrderId, order);
        return Results.Created($"/api/orders/{order.OrderId}", order);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.Run();

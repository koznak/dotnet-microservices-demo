using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var items = new List<CatalogItem>
{
    new(1, "SKU-COFFEE-001", "Colombian Coffee Beans", 18.90m, "USD"),
    new(2, "SKU-MUG-002", "Ceramic Mug", 11.50m, "USD"),
    new(3, "SKU-FRENCHPRESS-003", "French Press", 34.00m, "USD")
};

app.MapGet("/health", () => Results.Ok(new { service = "catalog", status = "healthy", timestampUtc = DateTime.UtcNow }));

app.MapGet("/api/catalog/items", () => Results.Ok(items));

app.MapGet("/api/catalog/items/{id:int}", (int id) =>
{
    var item = items.FirstOrDefault(x => x.Id == id);
    return item is null ? Results.NotFound(new { message = $"Catalog item '{id}' was not found." }) : Results.Ok(item);
});

app.Run();

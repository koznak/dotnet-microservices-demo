using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("catalog", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["Services:CatalogUrl"] ?? "http://localhost:5001");
});
builder.Services.AddHttpClient("orders", client =>
{
	client.BaseAddress = new Uri(builder.Configuration["Services:OrdersUrl"] ?? "http://localhost:5002");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { service = "gateway", status = "healthy", timestampUtc = DateTime.UtcNow }));

app.MapGet("/api/shop/catalog", async (IHttpClientFactory factory, CancellationToken ct) =>
{
	var client = factory.CreateClient("catalog");
	var result = await client.GetFromJsonAsync<List<CatalogItem>>("/api/catalog/items", ct);
	return Results.Ok(result ?? new List<CatalogItem>());
});

app.MapGet("/api/shop/orders", async (IHttpClientFactory factory, CancellationToken ct) =>
{
	var client = factory.CreateClient("orders");
	var result = await client.GetFromJsonAsync<List<OrderSummary>>("/api/orders", ct);
	return Results.Ok(result ?? new List<OrderSummary>());
});

app.MapPost("/api/shop/checkout", async (CreateOrderRequest request, IHttpClientFactory factory, CancellationToken ct) =>
{
	var client = factory.CreateClient("orders");
	var response = await client.PostAsJsonAsync("/api/orders", request, ct);
	var content = await response.Content.ReadAsStringAsync(ct);
	return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
});

app.MapGet("/", () => Results.Redirect("/api/shop/catalog"));

app.Run();

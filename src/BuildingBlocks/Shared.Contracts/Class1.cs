namespace Shared.Contracts;

public sealed record CatalogItem(int Id, string Sku, string Name, decimal Price, string Currency);

public sealed record OrderLineRequest(int CatalogItemId, int Quantity);

public sealed record CreateOrderRequest(string CustomerEmail, IReadOnlyList<OrderLineRequest> Lines);

public sealed record PricedOrderLine(int CatalogItemId, string Name, int Quantity, decimal UnitPrice, decimal LineTotal);

public sealed record OrderSummary(
	Guid OrderId,
	string CustomerEmail,
	DateTime CreatedUtc,
	IReadOnlyList<PricedOrderLine> Lines,
	decimal Subtotal,
	decimal Tax,
	decimal Total,
	string Currency);

public static class PricingEngine
{
	private const decimal TaxRate = 0.10m;

	public static OrderSummary BuildOrder(CreateOrderRequest request, IReadOnlyDictionary<int, CatalogItem> catalog)
	{
		ArgumentNullException.ThrowIfNull(request);
		ArgumentNullException.ThrowIfNull(catalog);

		if (string.IsNullOrWhiteSpace(request.CustomerEmail))
		{
			throw new ArgumentException("Customer email is required.", nameof(request));
		}

		if (request.Lines is null || request.Lines.Count == 0)
		{
			throw new ArgumentException("At least one order line is required.", nameof(request));
		}

		var pricedLines = new List<PricedOrderLine>(request.Lines.Count);

		foreach (var line in request.Lines)
		{
			if (line.Quantity <= 0)
			{
				throw new ArgumentException("Quantity must be greater than zero.", nameof(request));
			}

			if (!catalog.TryGetValue(line.CatalogItemId, out var item))
			{
				throw new ArgumentException($"Catalog item '{line.CatalogItemId}' was not found.", nameof(request));
			}

			var lineTotal = decimal.Round(item.Price * line.Quantity, 2, MidpointRounding.AwayFromZero);
			pricedLines.Add(new PricedOrderLine(item.Id, item.Name, line.Quantity, item.Price, lineTotal));
		}

		var subtotal = pricedLines.Sum(x => x.LineTotal);
		var tax = decimal.Round(subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
		var total = subtotal + tax;
		var currency = catalog.Values.FirstOrDefault()?.Currency ?? "USD";

		return new OrderSummary(Guid.NewGuid(), request.CustomerEmail, DateTime.UtcNow, pricedLines, subtotal, tax, total, currency);
	}
}

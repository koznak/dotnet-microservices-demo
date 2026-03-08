using Shared.Contracts;

namespace Services.Tests;

public class PricingEngineTests
{
    [Fact]
    public void BuildOrder_ReturnsExpectedTotals()
    {
        var catalog = new Dictionary<int, CatalogItem>
        {
            [1] = new(1, "SKU-1", "Test Item", 10.00m, "USD")
        };

        var request = new CreateOrderRequest("dev@example.com", [new OrderLineRequest(1, 2)]);

        var order = PricingEngine.BuildOrder(request, catalog);

        Assert.Equal(20.00m, order.Subtotal);
        Assert.Equal(2.00m, order.Tax);
        Assert.Equal(22.00m, order.Total);
        Assert.Single(order.Lines);
        Assert.Equal("USD", order.Currency);
    }

    [Fact]
    public void BuildOrder_Throws_WhenItemMissing()
    {
        var catalog = new Dictionary<int, CatalogItem>();
        var request = new CreateOrderRequest("dev@example.com", [new OrderLineRequest(999, 1)]);

        Assert.Throws<ArgumentException>(() => PricingEngine.BuildOrder(request, catalog));
    }
}

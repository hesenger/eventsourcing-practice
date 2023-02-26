namespace Esp.Tests;

public class AggregateRootTests
{
    private const string ValidOrderCreatedEventData = "{\"CreatedAt\": \"2023-02-26T11:29:35.917815-03:00\"}";

    [Test]
    public void InitializeShouldRehydrateEventStream()
    {
        var order = new Order();
        var @event = new Event(1, "Order", 1, 1, "Esp.Tests.OrderCreated", ValidOrderCreatedEventData, DateTime.Now);

        order.Initilize(1, new[] { @event });

        Assert.That(order.GetEventStream()[0], Is.EqualTo(@event));
    }

    [Test]
    public void InitializeShouldApplyEventToState()
    {
        var order = new Order();
        var @event = new Event(1, "Order", 1, 1, "Esp.Tests.OrderCreated", ValidOrderCreatedEventData, DateTime.Now);

        order.Initilize(42, new[] { @event });

        Assert.Multiple(() =>
        {
            Assert.That(order.Id, Is.EqualTo(42));
            Assert.That(order.CreatedAt, Is.EqualTo(DateTime.Parse("2023-02-26T11:29:35.917815-03:00")));
        });
    }

    [Test]
    public void PlaceOrderShouldStoreEventInStream()
    {
        var order = new Order();

        order.PlaceOrder(DateTime.Today);

        Assert.That(order.CreatedAt, Is.EqualTo(DateTime.Today));
    }
}

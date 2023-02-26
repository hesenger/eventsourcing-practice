namespace Esp.Tests;

public class AggregateRootTests
{
    [Test]
    public void ApplyEventShouldStoreEventInStream()
    {
        var order = new Order();
        var @event = new Event(1, "Order", 1, 1, "OrderCreated", "{}", DateTime.Now);

        order.ApplyEvent(@event);

        Assert.That(order.EventStream, Is.Not.Null);
        Assert.That(order.EventStream, Has.Count.EqualTo(1));
        Assert.That(order.EventStream[0], Is.EqualTo(@event));
    }
}

public class Order : IAggregateRoot
{
    public int Id { get; set; }
    public List<Event> EventStream { get; set; }

    public void ApplyEvent(Event @event)
    {
        throw new NotImplementedException();
    }
}

public interface IAggregateRoot
{
    int Id { get; }
    List<Event> EventStream { get; }

    void ApplyEvent(Event @event);
}

public record Event(
    int Id,
    string AggregateType,
    int AggregateId,
    int AggregateVersion,
    string EventType,
    string EventData,
    DateTime EventDate
);
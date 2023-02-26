namespace Esp.Tests;

public class AggregateRootTests
{
    [Test]
    public void InitializeShouldStoreEventInStream()
    {
        var order = new Order();
        var @event = new Event(1, "Order", 1, 1, "OrderCreated", "{}", DateTime.Now);

        order.Initilize(1, new[] { @event });

        Assert.That(order.GetEventStream(), Is.Not.Null);
        Assert.That(order.GetEventStream(), Has.Count.EqualTo(1));
        Assert.That(order.GetEventStream()[0], Is.EqualTo(@event));
    }
}

public class Order : IAggregateRoot
{
    private readonly List<Event> eventStream = new();

    public int Id { get; private set; }

    public IReadOnlyList<Event> GetEventStream() => eventStream;

    public void Initilize(int id, IEnumerable<Event> events)
    {
        Id = id;
        eventStream.AddRange(events);
        eventStream.ForEach(ApplyEvent);
    }

    public void ApplyEvent(Event @event)
    {
        switch (@event.EventType)
        {
            case "OrderCreated":
                break;
        }
    }

    public void PlaceOrder()
    {
        var @event = new Event(1, "Order", 1, 1, "OrderCreated", "{}", DateTime.Now);
        eventStream.Add(@event);
        ApplyEvent(@event);
    }
}

public interface IAggregateRoot
{
    int Id { get; }

    IReadOnlyList<Event> GetEventStream();

    void Initilize(int id, IEnumerable<Event> events);

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
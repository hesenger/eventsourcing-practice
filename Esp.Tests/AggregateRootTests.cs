using System.Text.Json;

namespace Esp.Tests;

public class AggregateRootTests
{
    [Test]
    public void InitializeShouldStoreEventInStream()
    {
        var order = new Order();
        var @event = new Event(1, "Order", 1, 1, "Esp.Tests.OrderCreated", "{\"CreateAt\": \"2021-01-01 08:00\"}", DateTime.Now);

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
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<Event> GetEventStream() => eventStream;

    public void Initilize(int id, IEnumerable<Event> events)
    {
        Id = id;
        foreach (var @event in events)
        {
            ApplyEvent((dynamic)@event.ToDomainEvent());
            eventStream.Add(@event);
        }
    }

    public void PlaceOrder(DateTime createdAt)
    {
        OrderCreated @event = new(createdAt);
        ApplyEvent(@event);
        eventStream.Add(Event.FromDomainEvent(this, @event));
    }

    private void ApplyEvent(OrderCreated @event)
    {
        CreatedAt = @event.CreatedAt;
    }
}

public record OrderCreated(DateTime CreatedAt) : IDomainEvent;

public interface IAggregateRoot
{
    int Id { get; }

    IReadOnlyList<Event> GetEventStream();

    void Initilize(int id, IEnumerable<Event> events);
}

public interface IDomainEvent
{
}

public record Event(
    int Id,
    string AggregateType,
    int AggregateId,
    int AggregateVersion,
    string EventType,
    string EventData,
    DateTime EventDate
)
{
    public IDomainEvent ToDomainEvent()
    {
        var type = Type.GetType(EventType)!;
        return (IDomainEvent)JsonSerializer.Deserialize(EventData, type)!;
    }

    public static Event FromDomainEvent(IAggregateRoot root, IDomainEvent domainEvent)
    {
        var type = domainEvent.GetType();
        var data = JsonSerializer.Serialize(domainEvent);
        return new Event(
            0,
            root.GetType().Name,
            root.Id,
            root.GetEventStream().Count + 1,
            type.FullName!,
            data,
            DateTime.Now);
    }
};

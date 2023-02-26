namespace Esp.Tests;

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

using System.Text.Json;

namespace Esp.Tests;

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

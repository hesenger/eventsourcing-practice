namespace Esp.Tests;

public interface IAggregateRoot
{
    int Id { get; }

    IReadOnlyList<Event> GetEventStream();

    void Initilize(int id, IEnumerable<Event> events);
}

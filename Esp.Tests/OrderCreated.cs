namespace Esp.Tests;

public record OrderCreated(DateTime CreatedAt) : IDomainEvent;

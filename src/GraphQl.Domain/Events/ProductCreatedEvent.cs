namespace GraphQl.Domain.Events;

public record ProductCreatedEvent(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    DateTime CreatedAt
);

namespace GraphQl.Domain.Events;

public record ProductUpdatedEvent(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    DateTime UpdatedAt
);

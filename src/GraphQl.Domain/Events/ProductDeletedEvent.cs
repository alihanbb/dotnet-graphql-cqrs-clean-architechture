namespace GraphQl.Domain.Events;

public record ProductDeletedEvent(
    string Id,
    DateTime DeletedAt
);

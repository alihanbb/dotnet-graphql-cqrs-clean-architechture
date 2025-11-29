namespace GraphQl.Domain.Constants;

public static class QueueNames
{
    // Queue names for consumers
    public const string ProductCreatedQueue = "product-created-queue";
    public const string ProductUpdatedQueue = "product-updated-queue";
    public const string ProductDeletedQueue = "product-deleted-queue";
}

public static class ExchangeNames
{
    // Exchange names for events
    public const string ProductEvents = "product-events";
}

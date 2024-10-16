namespace LoggingListener.Models;

internal sealed record EntityCreated
{
    /// <summary>
    ///     Gets the unique ID of the entity that was created.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the numeric ID of the entity that was created.
    /// </summary>
    public required int NumericId { get; init; }

    /// <summary>
    ///     Gets the time in UTC when the entity was created.
    /// </summary>
    public required DateTime Timestamp { get; init; }
}
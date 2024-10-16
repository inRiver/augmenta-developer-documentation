namespace LoggingListener.Models; 
internal sealed record LinkCreated
{
    /// <summary>
    ///     Gets the Unique ID of the link that was deleted.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the time in UTC when the link was created.
    /// </summary>
    public required DateTime Timestamp { get; init; }
}
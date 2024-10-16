namespace LoggingListener.Models; 
internal sealed record EntityUpdated
{
    /// <summary>
    ///     Gets the unique ID of the entity that was updated.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the numeric ID of the entity that was updated.
    /// </summary>
    public required int NumericId { get; init; }

    /// <summary>
    ///     Gets the fields that have been updated.
    /// </summary>
    public required FieldModelBase[] Fields { get; init; }

    /// <summary>
    ///     Gets the time in UTC when the entity was updated.
    /// </summary>
    public required DateTime Timestamp { get; init; }
}
namespace LoggingListener.Models; 
internal sealed record LinkDeleted
{
    /// <summary>
    ///     Gets the Unique ID of the link that was deleted.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the time in UTC when the link was deleted.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    ///     Gets the link type ID.
    /// </summary>
    public required string LinkTypeId { get; init; }

    /// <summary>
    ///     Gets the numeric source entity ID.
    /// </summary>
    public required int SourceEntityNumericId { get; init; }

    /// <summary>
    ///     Gets the source entity type ID.
    /// </summary>
    public required string SourceEntityTypeId { get; init; }

    /// <summary>
    ///     Gets the numeric target entity ID.
    /// </summary>
    public required int TargetEntityNumericId { get; init; }

    /// <summary>
    ///     Gets the target entity type ID.
    /// </summary>
    public required string TargetEntityTypeId { get; init; }

    /// <summary>
    ///     Gets the optional numeric link entity ID.
    /// </summary>
    public required int? LinkEntityNumericId { get; init; }

    /// <summary>
    ///     Gets the index.
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the link is inactive.
    /// </summary>
    public required bool Inactive { get; init; }
}
namespace LoggingListener.Models; 
internal sealed record EntityDeleted
{
    /// <summary>
    ///     Gets the Unique ID of the entity that was deleted.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the time in UTC when the entity was deleted.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    public required int ChangeSet { get; init; }
    public required int ContentSegmentationId { get; init; }
    public string? DisplayName { get; init; }
    public string? DisplayNameFieldTypeId { get; init; }
    public string? DisplayDescription { get; init; }
    public string? DisplayDescriptionFieldTypeId { get; init; }
    public required string EntityTypeId { get; init; }
    public string? FieldSetId { get; init; }
    public string? MainPictureUrl { get; init; }
    public string? Locked { get; init; }
    public int? MainPicture { get; init; }
    public required int NumericId { get; init; }
}
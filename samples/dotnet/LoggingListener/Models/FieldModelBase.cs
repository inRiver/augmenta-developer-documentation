namespace LoggingListener.Models; 
internal sealed record FieldModelBase
{
    /// <summary>
    ///     Gets the field type ID.
    /// </summary>
    public required string FieldTypeId { get; init; }
}
namespace EMerx.Common;

/// <summary>
/// Used for sending optional fields through DTO layer. Those fields have 3 operations:
/// - Don't do anything -> HasValue: false
/// - Replace => HasValue: true && Value != null
/// - Delete => HasValue: true && Value == null
/// </summary>
public struct OptionalField<T>
{
    private bool HasValue { get; }
    public T? Value { get; }

    public OptionalField(T? value)
    {
        HasValue = true;
        Value = value;
    }

    public bool IsDeleteOperation => HasValue && Value == null;
    public bool IsReplaceOperation => HasValue && Value != null;
}
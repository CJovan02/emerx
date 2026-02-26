using System.Text.Json.Serialization;

namespace EMerx.Common;

/// <summary>
/// Used for sending optional fields through DTO layer. Those fields have 3 operations:
/// - Don't do anything -> HasValue: false
/// - Replace => HasValue: true && Value != null
/// - Delete => HasValue: true && Value == null
/// </summary>
public class OptionalField<T>
{
    public bool HasValue { get; set; }
    public T? Value { get; set; }

    public OptionalField()
    {
    }

    public OptionalField(T? value)
    {
        HasValue = true;
        Value = value;
    }

    public bool IsDeleteOperation() => HasValue && Value == null;
    public bool IsReplaceOperation() => HasValue && Value != null;
    public bool IsNothing() => !HasValue;
}
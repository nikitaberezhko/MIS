namespace MIS.Api.Domain.ValueObjects;

public class Sex : ValueObject
{
    private Sex()
    {
        
    }
    public Sex(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Sex value cannot be null or empty", nameof(value));
        
        var normalizedValue = value.Trim().ToUpperInvariant();

        Value = normalizedValue;
    }
    
    public static Sex FromString(string value)
    {
        return new Sex(value);
    }

    public string Value { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
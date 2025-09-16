namespace MIS.Api.Domain.ValueObjects;

public class MedicalRecordNumber : ValueObject
{
    private MedicalRecordNumber() { }
    
    public MedicalRecordNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Medical record number cannot be null or empty", nameof(value));

        var normalizedValue = value.Trim().ToUpperInvariant();
        
        if (!IsValidMedicalRecordNumber(normalizedValue))
            throw new ArgumentException($"Invalid medical record number format: {value}", nameof(value));

        Value = normalizedValue;
    }

    public string Value { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static MedicalRecordNumber FromString(string value)
    {
        return new MedicalRecordNumber(value);
    }
    

    private static bool IsValidMedicalRecordNumber(string value)
    {
        // проверка по Regex или другая валидация

        return true;
    }

    public override string ToString()
    {
        return Value;
    }
}
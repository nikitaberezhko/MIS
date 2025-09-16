namespace MIS.Api.Domain.PatientAggregate;


public class BloodType
{
    private BloodType() { }
    public BloodType(int id, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Blood type value cannot be null or empty", nameof(value));

        var normalizedValue = value.Trim().ToUpperInvariant();
        
        if (!IsValidBloodType(normalizedValue))
            throw new ArgumentException($"Invalid blood type: {value}", nameof(value));

        Id = id;
        Value = normalizedValue;
    }

    public int Id { get; }
    public string Value { get; }

    public string BloodGroup => Value[..^1];
    public string RhFactor => Value[^1..];

    public static BloodType FromString(string value)
    {
        return GetBloodTypes().SingleOrDefault(b => b.Value == value) ??
               throw new ArgumentOutOfRangeException(nameof(value));
    }

    public static BloodType FromId(int id)
    {
        return GetBloodTypes().SingleOrDefault(b => b.Id == id) ??
               throw new ArgumentOutOfRangeException(nameof(id));
    }

    private static bool IsValidBloodType(string value)
    {
        var validBloodTypes = new[]
        {
            "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
        };

        return validBloodTypes.Contains(value);
    }
    
    public static IEnumerable<BloodType> GetBloodTypes()
    {
        return
        [
            APositive, ANegative, BPositive, BNegative, AbPositive, AbNegative, OPositive, ONegative
        ];
    }

    private static readonly BloodType APositive = new(1,"A+");
    private static readonly BloodType ANegative = new(2,"A-");
    private static readonly BloodType BPositive = new(3,"B+");
    private static readonly BloodType BNegative = new(4,"B-");
    private static readonly BloodType AbPositive = new(5,"AB+");
    private static readonly BloodType AbNegative = new(6,"AB-");
    private static readonly BloodType OPositive = new(7,"O+");
    private static readonly BloodType ONegative = new(8,"O-");
}
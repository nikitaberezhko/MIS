namespace MIS.Api.Domain.DoctorAggregate;


public class License
{
    private License(){}
    public License(string number, DateOnly validUntil)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("License number cannot be null or empty", nameof(number));
        if (!IsValidLicenseNumber(number))
            throw new ArgumentException($"Invalid license number format: {number}", nameof(number));
        if (validUntil < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("License cannot be expired", nameof(validUntil));

        Id = Guid.CreateVersion7();
        Number = number.Trim().ToUpperInvariant();
        ValidUntil = validUntil;
    }

    public Guid Id { get; }
    public string Number { get; }
    public DateOnly ValidUntil { get; }

    public bool IsExpired => ValidUntil < DateOnly.FromDateTime(DateTime.Today);
    public bool IsExpiringSoon => ValidUntil <= DateOnly.FromDateTime(DateTime.Today.AddDays(30));

    public int DaysUntilExpiration => (ValidUntil.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;
    
    public License Extend(DateOnly newValidUntil)
    {
        if (newValidUntil <= ValidUntil)
            throw new ArgumentException("New expiration date must be later than current expiration date", nameof(newValidUntil));

        return new License(Number, newValidUntil);
    }

    public License Extend(int additionalYears)
    {
        var newValidUntil = ValidUntil.AddYears(additionalYears);
        return Extend(newValidUntil);
    }

    public static License Create(string number, DateOnly validUntil)
    {
        return new License(number, validUntil);
    }

    public override string ToString() => $"{Number} (до {ValidUntil:dd.MM.yyyy})";

    private static bool IsValidLicenseNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return false;

        var trimmedNumber = number.Trim();
        
        // здесь проверка на валидность

        return true;
    }
}
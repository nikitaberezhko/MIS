namespace MIS.Api.Domain.ValueObjects;

public class Contacts : ValueObject
{
    private Contacts() { }
    
    public Contacts(string? phone = null, string? email = null)
    {
        if (phone != null && !IsValidPhone(phone))
            throw new ArgumentException($"Invalid phone number format: {phone}", nameof(phone));
        
        if (email != null && !IsValidEmail(email))
            throw new ArgumentException($"Invalid email format: {email}", nameof(email));

        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
    }

    public string? Phone { get; }

    public string? Email { get; }

    public bool HasPhone => !string.IsNullOrEmpty(Phone);
    public bool HasEmail => !string.IsNullOrEmpty(Email);
    public bool HasAnyContact => HasPhone || HasEmail;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Phone ?? string.Empty;
        yield return Email ?? string.Empty;
    }

    private static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        
        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
        
        return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString()
    {
        var parts = new List<string>();
        
        if (HasPhone)
            parts.Add($"Phone: {Phone}");
        
        if (HasEmail)
            parts.Add($"Email: {Email}");

        return parts.Count > 0 ? string.Join(", ", parts) : "No contacts";
    }
}
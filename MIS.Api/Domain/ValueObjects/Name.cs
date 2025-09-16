namespace MIS.Api.Domain.ValueObjects;

public class Name : ValueObject
{
    private Name() { }
    public Name(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim();
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string? MiddleName { get; }

    public string FullName => MiddleName != null 
        ? $"{LastName} {FirstName} {MiddleName}"
        : $"{LastName} {FirstName}";

    public string ShortName => $"{FirstName} {LastName}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        if (MiddleName != null)
            yield return MiddleName;
    }
    
    public static Name FromFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be null or empty", nameof(fullName));

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length < 2)
            throw new ArgumentException("Full name must contain at least first and last name", nameof(fullName));

        if (parts.Length == 2)
            return new Name(parts[1], parts[0]); // LastName FirstName

        if (parts.Length == 3)
            return new Name(parts[1], parts[0], parts[2]); // LastName FirstName MiddleName

        // For more than 3 parts, treat everything after first two as middle name
        var middleName = string.Join(" ", parts.Skip(2));
        return new Name(parts[1], parts[0], middleName);
    }
}
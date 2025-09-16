namespace MIS.Api.Domain.ValueObjects;

public class Address : ValueObject
{
    private Address() { }
    
    public Address(string addressLine, string city, string region, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new ArgumentException("Address line cannot be null or empty", nameof(addressLine));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be null or empty", nameof(postalCode));

        AddressLine = addressLine.Trim().ToUpperInvariant();
        City = city.Trim().ToUpperInvariant();
        Region = region.Trim().ToUpperInvariant();
        PostalCode = postalCode.Trim().ToLowerInvariant();
    }

    public string AddressLine { get; }

    public string City { get; }

    public string Region { get; }

    public string PostalCode { get; }

    public string FullAddress => $"{AddressLine}, {City}, {Region} {PostalCode}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine;
        yield return City;
        yield return Region;
        yield return PostalCode;
    }
}
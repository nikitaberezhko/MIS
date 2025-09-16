namespace MIS.Api.Domain.DiseaseAggregate;

public class Disease
{
    private Disease() { }
    
    public Disease(string name, string? description, bool isChronic, bool isInfectious)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Disease name cannot be null or empty", nameof(name));
        

        Id = Guid.CreateVersion7();
        Name = name.Trim();
        Description = description?.Trim();
        IsChronic = isChronic;
        IsInfectious = isInfectious;
    }

    public Guid Id { get; }
    public string Name { get; private set; } 
    public string? Description { get; private set; }

    public bool IsChronic { get; private set; }
    public bool IsInfectious { get; private set; }

    public override string ToString() => Name;
}



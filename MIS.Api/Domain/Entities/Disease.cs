using MIS.Api.Domain.Enums;

namespace MIS.Api.Domain.Entities;

public class Disease
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty; // e.g., ICD-10
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DiseaseSeverity DefaultSeverity { get; set; } = DiseaseSeverity.Unknown;

    public bool IsChronic { get; set; }
    public bool IsInfectious { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<PatientDisease> Patients { get; set; } = new List<PatientDisease>();
}



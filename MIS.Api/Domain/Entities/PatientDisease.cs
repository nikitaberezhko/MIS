using MIS.Api.Domain.Enums;

namespace MIS.Api.Domain.Entities;

public class PatientDisease
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DiseaseId { get; set; }
    public Disease Disease { get; set; } = null!;

    public DateOnly DiagnosedOn { get; set; }
    public DateOnly? ResolvedOn { get; set; }
    public DiagnosisStatus Status { get; set; } = DiagnosisStatus.Active;
    public DiseaseSeverity Severity { get; set; } = DiseaseSeverity.Unknown;
    public string? Notes { get; set; }
}


using MIS.Api.Domain.Enums;

namespace MIS.Api.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }

    public string MedicalRecordNumber { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    public Sex Sex { get; set; } = Sex.Unknown;

    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }

    public string? InsurancePolicyNumber { get; set; }
    public string? BloodType { get; set; }

    public Guid? PrimaryDoctorId { get; set; }
    public Doctor? PrimaryDoctor { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<PatientDisease> Diagnoses { get; set; } = new List<PatientDisease>();
}



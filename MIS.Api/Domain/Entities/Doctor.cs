using MIS.Api.Domain.Enums;

namespace MIS.Api.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }

    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    public DoctorSpecialty Specialty { get; set; } = DoctorSpecialty.GeneralPractitioner;
    public string? LicenseNumber { get; set; }
    public DateOnly? LicenseValidUntil { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}



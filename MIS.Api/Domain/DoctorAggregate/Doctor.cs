using MIS.Api.Domain.ValueObjects;

namespace MIS.Api.Domain.DoctorAggregate;

public class Doctor
{
    private Doctor() { }
    
    public Doctor(Name name, Contacts contacts, Specialty specialty, License? license = null, bool isActive = true)
    {
        Id = Guid.CreateVersion7();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Contacts = contacts ?? throw new ArgumentNullException(nameof(contacts));
        Specialty = specialty ?? throw new ArgumentNullException(nameof(specialty));
        License = license ?? throw new ArgumentNullException(nameof(license));
        IsActive = isActive;
    }


    public Guid Id { get; }

    public Name Name { get; private set; }
    public Contacts Contacts { get; private set; }

    public Specialty Specialty { get; private set; } = Specialty.GeneralPractitioner;
    public License License { get; private set; }

    public bool IsActive { get; private set; }

    public override string ToString() => Name.ToString();
}



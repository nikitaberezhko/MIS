using MIS.Api.Domain.ValueObjects;

namespace MIS.Api.Domain.PatientAggregate;

public class Patient
{
    private Patient() { }
    
    public Patient(MedicalRecordNumber medicalRecordNumber, Name name, Address address, BloodType bloodType, Contacts contacts, Sex sex, Guid? primaryDoctorId = null)
    {
        Id = Guid.CreateVersion7();
        MedicalRecordNumber = medicalRecordNumber ?? throw new ArgumentNullException(nameof(medicalRecordNumber));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        BloodType = bloodType ?? throw new ArgumentNullException(nameof(bloodType));
        Contacts = contacts ?? throw new ArgumentNullException(nameof(contacts));
        Sex = sex ?? throw new ArgumentNullException(nameof(sex));
        PrimaryDoctorId = primaryDoctorId;
    }
    
    public Guid Id { get; }
    public MedicalRecordNumber MedicalRecordNumber { get; private set; }
    public Name Name { get; private set; }
    public Address Address { get; private set; }
    public BloodType BloodType { get; private set; }
    public Contacts Contacts { get; private set; }
    public Sex Sex { get; private set; }
    public Guid? PrimaryDoctorId { get; private set; }

    public override string ToString() => Name.ToString();
}



namespace MIS.Api.Domain.DoctorAggregate;


public class Specialty
{
    private Specialty() { }
    
    public Specialty(int id, string value, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Specialty value cannot be null or empty", nameof(value));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be null or empty", nameof(displayName));

        Id = id;
        Value = value.Trim();
        DisplayName = displayName.Trim();
    }

    public int Id { get; }
    public string Value { get; }
    public string DisplayName { get; }
    

    public static Specialty FromString(string value)
    {
        var specialty = GetSpecialtyByValue(value);
        if (specialty == null)
            throw new ArgumentException($"Invalid doctor specialty: {value}", nameof(value));

        return specialty;
    }

    public static Specialty FromDisplayName(string displayName)
    {
        var specialty = GetSpecialtyByDisplayName(displayName);
        if (specialty == null)
            throw new ArgumentException($"Invalid doctor specialty display name: {displayName}", nameof(displayName));

        return specialty;
    }

    public static Specialty FromId(int id)
    {
        var specialty = AllSpecialties().FirstOrDefault(s => s.Id == id);
        if (specialty == null)
            throw new ArgumentException($"Invalid doctor specialty ID: {id}", nameof(id));

        return specialty;
    }

    public override string ToString() => DisplayName;

    private static Specialty? GetSpecialtyByValue(string value)
    {
        var normalizedValue = value.Trim().ToLowerInvariant();
        
        return AllSpecialties().FirstOrDefault(s => 
            s.Value.Equals(normalizedValue, StringComparison.OrdinalIgnoreCase));
    }

    private static Specialty? GetSpecialtyByDisplayName(string displayName)
    {
        var normalizedDisplayName = displayName.Trim();
        
        return AllSpecialties().FirstOrDefault(s => 
            s.DisplayName.Equals(normalizedDisplayName, StringComparison.OrdinalIgnoreCase));
    }

    // Предопределенные специализации
    public static readonly Specialty GeneralPractitioner = new(1,"general_practitioner", "Врач общей практики");
    public static readonly Specialty Cardiologist = new(2,"cardiologist", "Кардиолог");
    public static readonly Specialty Neurologist = new(3,"neurologist", "Невролог");
    public static readonly Specialty Dermatologist = new(4,"dermatologist", "Дерматолог");
    public static readonly Specialty Orthopedist = new(5,"orthopedist", "Ортопед");
    public static readonly Specialty Pediatrician = new(6,"pediatrician", "Педиатр");
    public static readonly Specialty Gynecologist = new(7,"gynecologist", "Гинеколог");
    public static readonly Specialty Ophthalmologist = new(8,"ophthalmologist", "Офтальмолог");
    public static readonly Specialty Psychiatrist = new(9,"psychiatrist", "Психиатр");
    public static readonly Specialty Surgeon = new(10,"surgeon", "Хирург");
    public static readonly Specialty Anesthesiologist = new(11,"anesthesiologist", "Анестезиолог");
    public static readonly Specialty Radiologist = new(12,"radiologist", "Рентгенолог");
    public static readonly Specialty Endocrinologist = new(13,"endocrinologist", "Эндокринолог");
    public static readonly Specialty Gastroenterologist = new(14,"gastroenterologist", "Гастроэнтеролог");
    public static readonly Specialty Pulmonologist = new(15,"pulmonologist", "Пульмонолог");
    public static readonly Specialty Urologist = new(16,"urologist", "Уролог");
    public static readonly Specialty Oncologist = new(17,"oncologist", "Онколог");
    public static readonly Specialty Rheumatologist = new(18,"rheumatologist", "Ревматолог");
    public static readonly Specialty Nephrologist = new(19,"nephrologist", "Нефролог");
    public static readonly Specialty Hematologist = new(20,"hematologist", "Гематолог");

    public static IReadOnlyList<Specialty> AllSpecialties() =>
    [
        GeneralPractitioner,
        Cardiologist,
        Neurologist,
        Dermatologist,
        Orthopedist,
        Pediatrician,
        Gynecologist,
        Ophthalmologist,
        Psychiatrist,
        Surgeon,
        Anesthesiologist,
        Radiologist,
        Endocrinologist,
        Gastroenterologist,
        Pulmonologist,
        Urologist,
        Oncologist,
        Rheumatologist,
        Nephrologist,
        Hematologist
    ];

}
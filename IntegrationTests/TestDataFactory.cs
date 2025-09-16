using MIS.Api.Domain.DiseaseAggregate;
using MIS.Api.Domain.DoctorAggregate;
using MIS.Api.Domain.PatientAggregate;
using MIS.Api.Domain.ValueObjects;

namespace IntegrationTests;

public static class TestDataFactory
{
    private static readonly Random Random = new();
    
    public static List<Disease> CreateTestDiseases(int count = 5)
    {
        var diseases = new List<Disease>();
        var diseaseNames = new[]
        {
            "Гипертония", "Диабет", "Астма", "Грипп", "Пневмония",
            "Гастрит", "Язва желудка", "Артрит", "Остеопороз", "Мигрень",
            "Депрессия", "Тревожное расстройство", "Аллергия", "Экзема", "Псориаз"
        };

        for (var i = 0; i < count; i++)
        {
            var name = diseaseNames[i % diseaseNames.Length];
            var isChronic = Random.Next(2) == 1;
            var isInfectious = Random.Next(2) == 1;

            diseases.Add(new Disease(
                name,
                $"Описание болезни: {name}",
                isChronic,
                isInfectious
            ));
        }

        return diseases;
    }


    public static List<Doctor> CreateTestDoctors(int count = 1, Specialty? specialty = null, bool isActive = true)
    {
        var doctors = new List<Doctor>();
        var specialties = Specialty.AllSpecialties();
        var firstNames = new[] { "Иван", "Петр", "Анна", "Мария", "Алексей", "Елена", "Дмитрий", "Ольга" };
        var lastNames = new[] { "Иванов", "Петров", "Сидоров", "Козлов", "Новиков", "Морозов", "Волков", "Соколов" };

        for (var i = 0; i < count; i++)
        {
            var selectedSpecialty = specialty ?? specialties[Random.Next(specialties.Count)];
            var firstName = firstNames[Random.Next(firstNames.Length)];
            var lastName = lastNames[Random.Next(lastNames.Length)];

            var doctor = new Doctor(new Name(firstName, lastName, null),
                new Contacts($"+7-{Random.Next(900, 999)}-{Random.Next(100, 999)}-{Random.Next(10, 99)}-{Random.Next(10, 99)}", $"doctor{i}@test.com"),
                selectedSpecialty,
                new License($"LIC-{Random.Next(100000, 999999)}", DateOnly.FromDateTime(DateTime.Today.AddYears(5))),
                isActive
            );

            doctors.Add(doctor);
        }

        return doctors;
    }
    
    public static List<Patient> CreateTestPatients(int count = 5)
    {
        var patients = new List<Patient>();
        var firstNames = new[] { "Алексей", "Елена", "Дмитрий", "Ольга", "Сергей", "Татьяна", "Андрей", "Наталья" };
        var lastNames = new[] { "Смирнов", "Кузнецов", "Попов", "Васильев", "Петров", "Соколов", "Михайлов", "Новиков" };
        var middleNames = new[] { "Александрович", "Владимирович", "Сергеевич", "Андреевич", "Александровна", "Владимировна", "Сергеевна", "Андреевна" };
        var cities = new[] { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Казань", "Нижний Новгород", "Челябинск", "Самара" }; 

        for (var i = 0; i < count; i++)
        {
            var firstName = firstNames[Random.Next(firstNames.Length)];
            var lastName = lastNames[Random.Next(lastNames.Length)];
            var middleName = middleNames[Random.Next(middleNames.Length)];
            var city = cities[Random.Next(cities.Length)];
            var bloodType = BloodType.GetBloodTypes().Single(x => x.Value == "A+");
            var sex = new Sex("FEMALE");

            var patient = new Patient(
                new MedicalRecordNumber($"MRN-{Random.Next(100000, 999999)}"),
                new Name(firstName, lastName, middleName),
                new Address($"ул. Тестовая, д. {Random.Next(1, 100)}", city, "Тестовая область", $"{Random.Next(100000, 999999)}"),
                bloodType,
                new Contacts( $"+7-{Random.Next(900, 999)}-{Random.Next(100, 999)}-{Random.Next(10, 99)}-{Random.Next(10, 99)}", $"patient{i}@test.com"),
                sex 
            );

            patients.Add(patient);
        }

        return patients;
    }
}
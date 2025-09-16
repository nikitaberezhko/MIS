using System.Net;
using System.Net.Http.Json;
using MIS.Api.Controllers.Contract.OpenApi.Models;
using Patient = MIS.Api.Domain.PatientAggregate.Patient;

namespace IntegrationTests;


public class PatientControllerTests: IntegrationTestBase
{
    [Fact]
    public async Task GetAllPatients_ShouldReturnOkWithPatients_WhenPatientsExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var patients = CreateTestPatients();
        await AddPatientsToDatabaseAsync(patients);

        // Act
        var response = await Client.GetAsync("/patients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllPatients200Response>();
        Assert.NotNull(result);
        Assert.Equal(patients.Count, result!.Patients.Count);
        Assert.NotNull(result.Pagination);
        Assert.Equal(patients.Count, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetAllPatients_ShouldReturnEmptyList_WhenNoPatientsExist()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/patients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllPatients200Response>();
        Assert.NotNull(result);
        Assert.Empty(result!.Patients);
        Assert.Equal(0, result.Pagination.TotalCount);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 3)]
    [InlineData(1, 10)]
    public async Task GetAllPatients_ShouldReturnCorrectPagination_WhenPaginationParametersProvided(int page, int pageSize)
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var patients = CreateTestPatients(15); // Создаем 15 пациентов
        await AddPatientsToDatabaseAsync(patients);

        // Act
        var response = await Client.GetAsync($"/patients?page={page}&pageSize={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllPatients200Response>();
        Assert.NotNull(result);
        Assert.Equal(Math.Min(pageSize, Math.Max(0, patients.Count - (page - 1) * pageSize)), result!.Patients.Count);
        Assert.NotNull(result.Pagination);
        Assert.Equal(page, result.Pagination.Page);
        Assert.Equal(pageSize, result.Pagination.PageSize);
        Assert.Equal(patients.Count, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetAllPatients_ShouldReturnCorrectApiModel_WhenPatientsExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var patients = CreateTestPatients(1);
        await AddPatientsToDatabaseAsync(patients);

        // Act
        var response = await Client.GetAsync("/patients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllPatients200Response>();
        Assert.NotNull(result);
        
        var apiPatient = result!.Patients.First();
        var domainPatient = patients.First();
        
        Assert.Equal(domainPatient.Id, apiPatient.Id);
        Assert.Equal(domainPatient.MedicalRecordNumber.Value, apiPatient.MedicalRecordNumber);
        Assert.Equal(domainPatient.Name.FirstName, apiPatient.Name.FirstName);
        Assert.Equal(domainPatient.Name.LastName, apiPatient.Name.LastName);
        Assert.Equal(domainPatient.Address.AddressLine, apiPatient.Address.AddressLine);
        Assert.Equal(domainPatient.Address.City, apiPatient.Address.City);
        Assert.Equal(MIS.Api.Controllers.Contract.OpenApi.Models.Patient.BloodTypeEnum.AEnum.ToString(), apiPatient.BloodType.ToString());
        Assert.Equal(domainPatient.Contacts.Email, apiPatient.Contacts.Email);
        Assert.Equal(domainPatient.Contacts.Phone, apiPatient.Contacts.Phone);
        Assert.Equal(domainPatient.Sex.Value, apiPatient.Sex);
        Assert.Equal(domainPatient.PrimaryDoctorId, apiPatient.PrimaryDoctorId);
    }

    [Fact]
    public async Task GetPatientById_ShouldReturnOkWithPatient_WhenPatientExists()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var patients = CreateTestPatients(1);
        await AddPatientsToDatabaseAsync(patients);
        var patientId = patients.First().Id;

        // Act
        var response = await Client.GetAsync($"/patients/{patientId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<MIS.Api.Controllers.Contract.OpenApi.Models.Patient>();
        Assert.NotNull(result);
        Assert.Equal(patientId, result!.Id);
    }

    [Fact]
    public async Task GetPatientById_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/patients/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
        Assert.Equal("Patient not found", result.Description);
    }

    [Fact]
    public async Task GetPatientById_ShouldReturnCorrectApiModel_WhenPatientExists()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var patients = CreateTestPatients(1);
        await AddPatientsToDatabaseAsync(patients);
        var patientId = patients.First().Id;

        // Act
        var response = await Client.GetAsync($"/patients/{patientId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<MIS.Api.Controllers.Contract.OpenApi.Models.Patient>();
        Assert.NotNull(result);
        
        var apiPatient = result!;
        var domainPatient = patients.First();
        
        Assert.Equal(domainPatient.Id, apiPatient.Id);
        Assert.Equal(domainPatient.MedicalRecordNumber.Value, apiPatient.MedicalRecordNumber);
        Assert.Equal(domainPatient.Name.FirstName, apiPatient.Name.FirstName);
        Assert.Equal(domainPatient.Name.LastName, apiPatient.Name.LastName);
        Assert.Equal(domainPatient.Name.MiddleName, apiPatient.Name.MiddleName);
        Assert.Equal(domainPatient.Address.AddressLine, apiPatient.Address.AddressLine);
        Assert.Equal(domainPatient.Address.City, apiPatient.Address.City);
        Assert.Equal(domainPatient.Address.Region, apiPatient.Address.Region);
        Assert.Equal(domainPatient.Address.PostalCode, apiPatient.Address.PostalCode);
        Assert.Equal(MIS.Api.Controllers.Contract.OpenApi.Models.Patient.BloodTypeEnum.AEnum.ToString(), apiPatient.BloodType.ToString());
        Assert.Equal(domainPatient.Contacts.Email, apiPatient.Contacts.Email);
        Assert.Equal(domainPatient.Contacts.Phone, apiPatient.Contacts.Phone);
        Assert.Equal(domainPatient.Sex.Value, apiPatient.Sex);
        Assert.Equal(domainPatient.PrimaryDoctorId, apiPatient.PrimaryDoctorId);
    }

    [Fact]
    public async Task GetPatientById_ShouldReturnBadRequest_WhenInvalidGuidProvided()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/patients/invalid-guid");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    private List<Patient> CreateTestPatients(int count = 5)
    {
        return TestDataFactory.CreateTestPatients(count);
    }

    private async Task AddPatientsToDatabaseAsync(List<Patient> patients)
    {
        await DbContext.Patients.AddRangeAsync(patients);
        foreach (var patient in patients)
        {
            DbContext.Attach(patient.BloodType);
        }
        
        await DbContext.SaveChangesAsync();
    }
    
}
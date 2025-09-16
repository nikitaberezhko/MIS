using System.Net;
using System.Net.Http.Json;
using MIS.Api.Controllers.Contract.OpenApi.Models;
using MIS.Api.Domain.DoctorAggregate;
using Doctor = MIS.Api.Domain.DoctorAggregate.Doctor;

namespace IntegrationTests;

public class DoctorControllerTests : IntegrationTestBase
{

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldReturnOkWithDoctors_WhenDoctorsExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var doctors = CreateTestDoctors();
        await AddDoctorsToDatabaseAsync(doctors);

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=cardiologist");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsBySpecialty200Response>();
        Assert.NotNull(result);
        Assert.Equal(doctors.Count(d => d.Specialty.Value == "cardiologist"), result!.Doctors.Count);
        Assert.NotNull(result.Pagination);
        Assert.Equal(doctors.Count(d => d.Specialty.Value == "cardiologist"), result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldReturnEmptyList_WhenNoDoctorsWithSpecialtyExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var doctors = CreateTestDoctors();
        await AddDoctorsToDatabaseAsync(doctors);

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=oncologist");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsBySpecialty200Response>();
        Assert.NotNull(result);
        Assert.Empty(result!.Doctors);
        Assert.Equal(0, result.Pagination.TotalCount);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 3)]
    [InlineData(1, 10)]
    public async Task GetDoctorsBySpecialty_ShouldReturnCorrectPagination_WhenPaginationParametersProvided(int page, int pageSize)
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var doctors = CreateTestDoctors(15, Specialty.Cardiologist);
        await AddDoctorsToDatabaseAsync(doctors);

        // Act
        var response = await Client.GetAsync($"/doctors/by-specialty?specialty=cardiologist&page={page}&pageSize={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsBySpecialty200Response>();
        Assert.NotNull(result);
        Assert.Equal(Math.Min(pageSize, Math.Max(0, doctors.Count - (page - 1) * pageSize)), result!.Doctors.Count);
        Assert.NotNull(result.Pagination);
        Assert.Equal(page, result.Pagination.Page);
        Assert.Equal(pageSize, result.Pagination.PageSize);
        Assert.Equal(doctors.Count, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldReturnBadRequest_WhenSpecialtyIsEmpty()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldReturnBadRequest_WhenSpecialtyIsInvalid()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=invalid_specialty");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
        Assert.Contains("Unknown specialty", result.Description);
    }

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldAcceptDisplayName_WhenDisplayNameProvided()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var doctors = CreateTestDoctors(1, Specialty.Cardiologist);
        await AddDoctorsToDatabaseAsync(doctors);

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=Кардиолог");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsBySpecialty200Response>();
        Assert.NotNull(result);
        Assert.Equal(doctors.Count, result!.Doctors.Count);
    }

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldOnlyReturnActiveDoctors_WhenInactiveDoctorsExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var activeDoctors = CreateTestDoctors(2, Specialty.Cardiologist, isActive: true);
        var inactiveDoctors = CreateTestDoctors(3, Specialty.Cardiologist, isActive: false);
        
        await AddDoctorsToDatabaseAsync(activeDoctors.Concat(inactiveDoctors).ToList());

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=cardiologist");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsBySpecialty200Response>();
        Assert.NotNull(result);
        Assert.Equal(activeDoctors.Count, result!.Doctors.Count);
        Assert.All(result.Doctors, d => Assert.True(d.IsActive));
    }

    [Fact]
    public async Task GetDoctorsBySpecialty_ShouldReturnCorrectApiModel_WhenDoctorsExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var doctors = CreateTestDoctors(1, Specialty.Cardiologist);
        await AddDoctorsToDatabaseAsync(doctors);

        // Act
        var response = await Client.GetAsync("/doctors/by-specialty?specialty=cardiologist");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsBySpecialty200Response>();
        Assert.NotNull(result);
        
        var apiDoctor = result!.Doctors.First();
        var domainDoctor = doctors.First();
        
        Assert.Equal(domainDoctor.Id, apiDoctor.Id);
        Assert.Equal(domainDoctor.Name.FirstName, apiDoctor.Name.FirstName);
        Assert.Equal(domainDoctor.Name.LastName, apiDoctor.Name.LastName);
        Assert.Equal(domainDoctor.Contacts.Email, apiDoctor.Contacts.Email);
        Assert.Equal(domainDoctor.Contacts.Phone, apiDoctor.Contacts.Phone);
        Assert.Equal(domainDoctor.Specialty.Value, apiDoctor.Specialty.Value);
        Assert.Equal(domainDoctor.Specialty.DisplayName, apiDoctor.Specialty.DisplayName);
        Assert.Equal(domainDoctor.License.Number, apiDoctor.License.Number);
        Assert.Equal(domainDoctor.IsActive, apiDoctor.IsActive);
    }
    
    private List<Doctor> CreateTestDoctors(int count = 5, Specialty? specialty = null, bool isActive = true)
    {
        return TestDataFactory.CreateTestDoctors(count, specialty, isActive);
    }

    private async Task AddDoctorsToDatabaseAsync(List<Doctor> doctors)
    {
        foreach (var doctor in doctors)
        {
            DbContext.Add(doctor.License);
            DbContext.Attach(doctor.Specialty);
            DbContext.Add(doctor);
            
            await DbContext.SaveChangesAsync();
        }
    }
}
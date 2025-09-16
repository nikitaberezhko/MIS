using System.Net;
using System.Net.Http.Json;
using MIS.Api.Controllers.Contract.OpenApi.Models;
using Disease = MIS.Api.Domain.DiseaseAggregate.Disease;

namespace IntegrationTests;

public class DiseaseControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAllDiseases_ShouldReturnOkWithDiseases_WhenDiseasesExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var diseases = CreateTestDiseases();
        await AddDiseasesToDatabaseAsync(diseases);

        // Act
        var response = await Client.GetAsync("/diseases");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        Assert.Equal(diseases.Count, result!.Diseases.Count);
        Assert.NotNull(result.Pagination);
        Assert.Equal(diseases.Count, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetAllDiseases_ShouldReturnEmptyList_WhenNoDiseasesExist()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync("/diseases");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        Assert.Empty(result!.Diseases);
        Assert.Equal(0, result.Pagination.TotalCount);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 3)]
    [InlineData(1, 10)]
    public async Task GetAllDiseases_ShouldReturnCorrectPagination_WhenPaginationParametersProvided(int page, int pageSize)
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var diseases = CreateTestDiseases(15); // Создаем 15 болезней
        await AddDiseasesToDatabaseAsync(diseases);

        // Act
        var response = await Client.GetAsync($"/diseases?page={page}&pageSize={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        Assert.Equal(Math.Min(pageSize, Math.Max(0, diseases.Count - (page - 1) * pageSize)), result!.Diseases.Count);
        Assert.NotNull(result.Pagination);
        Assert.Equal(page, result.Pagination.Page);
        Assert.Equal(pageSize, result.Pagination.PageSize);
        Assert.Equal(diseases.Count, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetAllDiseases_ShouldFilterByIsChronic_WhenIsChronicParameterProvided()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var diseases = CreateTestDiseases();
        await AddDiseasesToDatabaseAsync(diseases);

        // Act
        var response = await Client.GetAsync("/diseases?isChronic=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        Assert.All(result!.Diseases, d => Assert.True(d.IsChronic));
    }

    [Fact]
    public async Task GetAllDiseases_ShouldFilterByIsInfectious_WhenIsInfectiousParameterProvided()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var diseases = CreateTestDiseases();
        await AddDiseasesToDatabaseAsync(diseases);

        // Act
        var response = await Client.GetAsync("/diseases?isInfectious=false");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        Assert.All(result!.Diseases, d => Assert.False(d.IsInfectious));
    }

    [Fact]
    public async Task GetAllDiseases_ShouldFilterByBothParameters_WhenBothParametersProvided()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var diseases = CreateTestDiseases();
        await AddDiseasesToDatabaseAsync(diseases);

        // Act
        var response = await Client.GetAsync("/diseases?isChronic=true&isInfectious=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        Assert.All(result!.Diseases, d => Assert.True(d.IsChronic && d.IsInfectious));
    }

    [Fact]
    public async Task GetAllDiseases_ShouldReturnCorrectApiModel_WhenDiseasesExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        
        var diseases = CreateTestDiseases(1);
        await AddDiseasesToDatabaseAsync(diseases);

        // Act
        var response = await Client.GetAsync("/diseases");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<GetAllDiseases200Response>();
        Assert.NotNull(result);
        
        var apiDisease = result!.Diseases.First();
        var domainDisease = diseases.First();
        
        Assert.Equal(domainDisease.Id, apiDisease.Id);
        Assert.Equal(domainDisease.Name, apiDisease.Name);
        Assert.Equal(domainDisease.Description, apiDisease.Description);
        Assert.Equal(domainDisease.IsChronic, apiDisease.IsChronic);
        Assert.Equal(domainDisease.IsInfectious, apiDisease.IsInfectious);
    }
    
    private List<Disease> CreateTestDiseases(int count = 5)
    {
        return TestDataFactory.CreateTestDiseases(count);
    }

    private async Task AddDiseasesToDatabaseAsync(List<Disease> diseases)
    {
        await DbContext.Diseases.AddRangeAsync(diseases);
        
        await DbContext.SaveChangesAsync();
    }
}
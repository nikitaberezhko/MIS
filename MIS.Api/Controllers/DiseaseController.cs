using Microsoft.AspNetCore.Mvc;
using MIS.Api.Controllers.Contract.OpenApi.Controllers;
using MIS.Api.Controllers.Contract.OpenApi.Models;
using MIS.Api.Controllers.Shared;
using MIS.Api.Infrastructure.Repositories;

namespace MIS.Api.Controllers;

public class DiseaseController(DiseaseRepository diseaseRepository) : DiseasesApiController
{
    public override async Task<IActionResult> GetAllDiseases(int? page, int? pageSize, bool? isChronic, bool? isInfectious)
    {
        var totalCount = await diseaseRepository.GetCountAsync(isChronic, isInfectious);
        
        var (skip, take) = PaginationResolver.GetPaginationParams(page, pageSize);
        var diseases = await diseaseRepository.GetAllAsync(skip, take, isChronic, isInfectious);
        
        var apiDiseases = diseases.Select(d => d.ToApiModel()).ToList();
        
        var response = new GetAllDiseases200Response
        {
            Diseases = apiDiseases,
            Pagination = PaginationResolver.CreatePaginationInfo(page ?? 1, pageSize ?? 20, totalCount)
        };

        return Ok(response);
    }
}
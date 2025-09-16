using Microsoft.AspNetCore.Mvc;
using MIS.Api.Controllers.Contract.OpenApi.Controllers;
using MIS.Api.Controllers.Contract.OpenApi.Models;
using MIS.Api.Controllers.Shared;
using MIS.Api.Infrastructure.Repositories;

namespace MIS.Api.Controllers;

public class PatientController(PatientRepository patientRepository) : PatientsApiController
{
    public override async Task<IActionResult> GetAllPatients(int? page, int? pageSize)
    {
        var totalCount = await patientRepository.GetCountAsync();
        
        var (skip, take) = PaginationResolver.GetPaginationParams(page, pageSize);
        var patients = await patientRepository.GetAllAsync(skip, take);
        
        var apiPatients = patients.Select(p => p.ToApiModel()).ToList();
        
        var response = new GetAllPatients200Response
        {
            Patients = apiPatients,
            Pagination = PaginationResolver.CreatePaginationInfo(page ?? 1, pageSize ?? 20, totalCount)
        };

        return Ok(response);
    }

    public override async Task<IActionResult> GetPatientById(Guid patientId)
    {
        var patient = await patientRepository.GetByIdAsync(patientId);

        if (patient == null)
            return NotFound(new Error
            {
                StatusCode = 404,
                Description = $"Patient not found",
                Timestamp = DateTime.UtcNow,
                TraceId = HttpContext.TraceIdentifier
            });
        
        var apiPatient = patient.ToApiModel();

        return Ok(apiPatient);
    }
}
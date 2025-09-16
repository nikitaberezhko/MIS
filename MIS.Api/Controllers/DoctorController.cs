using Microsoft.AspNetCore.Mvc;
using MIS.Api.Controllers.Contract.OpenApi.Controllers;
using MIS.Api.Controllers.Contract.OpenApi.Models;
using MIS.Api.Controllers.Shared;
using MIS.Api.Domain.DoctorAggregate;
using MIS.Api.Infrastructure.Repositories;

namespace MIS.Api.Controllers;

public class DoctorController(DoctorRepository doctorRepository) : DoctorsApiController
{
    public override async Task<IActionResult> GetDoctorsBySpecialty(string specialty, int? page, int? pageSize)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return BadRequest(new Error
            {
                StatusCode = 400,
                Description = "Specialty is required",
                Timestamp = DateTime.UtcNow,
                TraceId = HttpContext.TraceIdentifier
            });
        
        Specialty? domainSpecialty;
        try
        {
            domainSpecialty = Specialty.FromString(specialty);
        }
        catch (ArgumentException)
        {
            try
            {
                domainSpecialty = Specialty.FromDisplayName(specialty);
            }
            catch (ArgumentException)
            {
                return BadRequest(new Error
                {
                    StatusCode = 400,
                    Description = $"Unknown specialty: {specialty}",
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        var totalCount = await doctorRepository.GetCountBySpecialtyAsync(domainSpecialty);
        
        var (skip, take) = PaginationResolver.GetPaginationParams(page, pageSize);
        var doctors = await doctorRepository.GetBySpecialtyAsync(domainSpecialty, skip, take);
        
        var apiDoctors = doctors.Select(d => d.ToApiModel()).ToList();
        
        var response = new GetDoctorsBySpecialty200Response
        {
            Doctors = apiDoctors,
            Pagination = PaginationResolver.CreatePaginationInfo(page ?? 1, pageSize ?? 20, totalCount)
        };

        return Ok(response);
    }
}
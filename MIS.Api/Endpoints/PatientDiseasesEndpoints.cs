using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.Entities;
using MIS.Api.Infrastructure;

namespace MIS.Api.Endpoints;

public static class PatientDiseasesEndpoints
{
    public static IEndpointRouteBuilder MapPatientDiseases(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/patients/{patientId:guid}/diseases").WithTags("PatientDiseases");

        group.MapGet("", async (Guid patientId, AppDbContext db) =>
        {
            var list = await db.PatientDiseases
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .Include(x => x.Disease)
                .OrderByDescending(x => x.DiagnosedOn)
                .ToListAsync();
            return Results.Ok(list);
        });

        group.MapPost("", async (Guid patientId, PatientDisease input, AppDbContext db) =>
        {
            var exists = await db.Patients.AnyAsync(p => p.Id == patientId);
            if (!exists) return Results.NotFound($"Patient {patientId} not found");

            input.PatientId = patientId;
            db.PatientDiseases.Add(input);
            await db.SaveChangesAsync();
            return Results.Created($"/api/patients/{patientId}/diseases/{input.DiseaseId}", input);
        });

        group.MapDelete("{diseaseId:guid}", async (Guid patientId, Guid diseaseId, AppDbContext db) =>
        {
            var entity = await db.PatientDiseases.FirstOrDefaultAsync(x => x.PatientId == patientId && x.DiseaseId == diseaseId);
            if (entity is null) return Results.NotFound();
            db.PatientDiseases.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}



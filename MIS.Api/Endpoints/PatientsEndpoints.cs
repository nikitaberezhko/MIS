using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.Entities;
using MIS.Api.Infrastructure;

namespace MIS.Api.Endpoints;

public static class PatientsEndpoints
{
    public static IEndpointRouteBuilder MapPatients(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/patients").WithTags("Patients");

        group.MapGet("", async (AppDbContext db) =>
        {
            var list = await db.Patients
                .AsNoTracking()
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .ToListAsync();
            return Results.Ok(list);
        });

        group.MapGet("{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var patient = await db.Patients
                .AsNoTracking()
                .Include(p => p.PrimaryDoctor)
                .Include(p => p.Diagnoses).ThenInclude(d => d.Disease)
                .FirstOrDefaultAsync(x => x.Id == id);
            return patient is null ? Results.NotFound() : Results.Ok(patient);
        });

        group.MapPost("", async (Patient input, AppDbContext db) =>
        {
            input.Id = Guid.NewGuid();
            db.Patients.Add(input);
            await db.SaveChangesAsync();
            return Results.Created($"/api/patients/{input.Id}", input);
        });

        group.MapPut("{id:guid}", async (Guid id, Patient input, AppDbContext db) =>
        {
            var entity = await db.Patients.FindAsync(id);
            if (entity is null) return Results.NotFound();

            db.Entry(entity).CurrentValues.SetValues(input);
            entity.Id = id;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var entity = await db.Patients.FindAsync(id);
            if (entity is null) return Results.NotFound();
            db.Patients.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}



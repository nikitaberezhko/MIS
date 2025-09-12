using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.Entities;
using MIS.Api.Infrastructure;

namespace MIS.Api.Endpoints;

public static class DoctorsEndpoints
{
    public static IEndpointRouteBuilder MapDoctors(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/doctors").WithTags("Doctors");

        group.MapGet("", async (AppDbContext db) =>
        {
            var list = await db.Doctors
                .AsNoTracking()
                .OrderBy(d => d.LastName).ThenBy(d => d.FirstName)
                .ToListAsync();
            return Results.Ok(list);
        });

        group.MapGet("{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var doctor = await db.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return doctor is null ? Results.NotFound() : Results.Ok(doctor);
        });

        group.MapPost("", async (Doctor input, AppDbContext db) =>
        {
            input.Id = Guid.NewGuid();
            db.Doctors.Add(input);
            await db.SaveChangesAsync();
            return Results.Created($"/api/doctors/{input.Id}", input);
        });

        group.MapPut("{id:guid}", async (Guid id, Doctor input, AppDbContext db) =>
        {
            var entity = await db.Doctors.FindAsync(id);
            if (entity is null) return Results.NotFound();

            db.Entry(entity).CurrentValues.SetValues(input);
            entity.Id = id;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var entity = await db.Doctors.FindAsync(id);
            if (entity is null) return Results.NotFound();
            db.Doctors.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}



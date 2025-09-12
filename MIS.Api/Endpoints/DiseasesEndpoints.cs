using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.Entities;
using MIS.Api.Infrastructure;

namespace MIS.Api.Endpoints;

public static class DiseasesEndpoints
{
    public static IEndpointRouteBuilder MapDiseases(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/diseases").WithTags("Diseases");

        group.MapGet("", async (AppDbContext db) =>
        {
            var list = await db.Diseases
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();
            return Results.Ok(list);
        });

        group.MapGet("{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var disease = await db.Diseases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return disease is null ? Results.NotFound() : Results.Ok(disease);
        });

        group.MapPost("", async (Disease input, AppDbContext db) =>
        {
            input.Id = Guid.NewGuid();
            db.Diseases.Add(input);
            await db.SaveChangesAsync();
            return Results.Created($"/api/diseases/{input.Id}", input);
        });

        group.MapPut("{id:guid}", async (Guid id, Disease input, AppDbContext db) =>
        {
            var entity = await db.Diseases.FindAsync(id);
            if (entity is null) return Results.NotFound();

            db.Entry(entity).CurrentValues.SetValues(input);
            entity.Id = id;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var entity = await db.Diseases.FindAsync(id);
            if (entity is null) return Results.NotFound();
            db.Diseases.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}



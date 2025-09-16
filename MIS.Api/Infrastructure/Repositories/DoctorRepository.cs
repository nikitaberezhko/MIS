using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.DoctorAggregate;

namespace MIS.Api.Infrastructure.Repositories;

public class DoctorRepository(AppDbContext context)
{
    public async Task<IEnumerable<Doctor>> GetBySpecialtyAsync(Specialty specialty, int skip, int take)
    {
        return await context.Doctors
            .Include(x => x.License)
            .Include(x => x.Specialty)
            .Where(d => d.Specialty.Id == specialty.Id && d.IsActive)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetCountBySpecialtyAsync(Specialty specialty)
    {
        return await context.Doctors
            .Where(d => d.Specialty.Id == specialty.Id && d.IsActive)
            .CountAsync();
    }
}
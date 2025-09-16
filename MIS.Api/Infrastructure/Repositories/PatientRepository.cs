using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.PatientAggregate;

namespace MIS.Api.Infrastructure.Repositories;

public class PatientRepository(AppDbContext context)
{
    public async Task<IEnumerable<Patient>> GetAllAsync(int skip, int take)
    {
        return await context.Patients
            .Include(x => x.BloodType)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await context.Patients
            .Include(x => x.BloodType)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Patients.CountAsync();
    }
}
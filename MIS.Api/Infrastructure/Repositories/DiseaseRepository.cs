using Microsoft.EntityFrameworkCore;
using MIS.Api.Domain.DiseaseAggregate;

namespace MIS.Api.Infrastructure.Repositories;

public class DiseaseRepository(AppDbContext context)
{
    public async Task<IEnumerable<Disease>> GetAllAsync(int skip, int take, bool? isChronic = null, bool? isInfectious = null)
    {
        var query = context.Diseases.AsQueryable();
        
        if (isChronic.HasValue) 
            query = query.Where(d => d.IsChronic == isChronic.Value);
        if (isInfectious.HasValue) 
            query = query.Where(d => d.IsInfectious == isInfectious.Value);
        
        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(bool? isChronic = null, bool? isInfectious = null)
    {
        var query = context.Diseases.AsQueryable();
        
        if (isChronic.HasValue) 
            query = query.Where(d => d.IsChronic == isChronic.Value);
        if (isInfectious.HasValue) 
            query = query.Where(d => d.IsInfectious == isInfectious.Value);
        
        return await query.CountAsync();
    }
}
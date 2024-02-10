using Microsoft.EntityFrameworkCore;
using NZWalks.Data;
using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;

public class SqlWalkRepository : IWalkRepository
{
    private readonly NzWalksDbContext _dbContext;

    public SqlWalkRepository(NzWalksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Walk> CreateAsync(Walk walk)
    {
        await _dbContext.Walks.AddAsync(walk);
        await _dbContext.SaveChangesAsync();
        return walk;
    }

    public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
        string? sortBy = null,
        bool isAscending = true)
    {
        var walks = _dbContext.Walks
            .Include("Difficulty")
            .Include("Region")
            .AsQueryable();

        //Filtering
        if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            walks = filterOn.ToLower() switch
            {
                "name" => walks.Where(w => w.Name.Contains(filterQuery)),
                "region" => walks.Where(w => w.Region.Name.Contains(filterQuery)),
                "difficulty" => walks.Where(w => w.Difficulty.Name.Contains(filterQuery)),
                _ => walks
            };
        //Sorting, include lengthKm
        if (!string.IsNullOrWhiteSpace(sortBy))
            walks = sortBy.ToLower() switch
            {
                "name" => isAscending ? walks.OrderBy(w => w.Name) : walks.OrderByDescending(w => w.Name),
                "region" => isAscending
                    ? walks.OrderBy(w => w.Region.Name)
                    : walks.OrderByDescending(w => w.Region.Name),
                "difficulty" => isAscending
                    ? walks.OrderBy(w => w.Difficulty.Name)
                    : walks.OrderByDescending(w => w.Difficulty.Name),
                "lengthkm" => isAscending ? walks.OrderBy(w => w.LengthKm) : walks.OrderByDescending(w => w.LengthKm),
                _ => walks
            };


        return await walks.ToListAsync();
    }

    public async Task<Walk?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Walks
            .Include("Difficulty")
            .Include("Region")
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
    {
        var existingWalk = await _dbContext.Walks.FirstOrDefaultAsync(w => w.Id == id);
        if (existingWalk == null) return null;

        existingWalk.Name = walk.Name;
        existingWalk.Description = walk.Description;
        existingWalk.LengthKm = walk.LengthKm;
        existingWalk.WalkImageUrl = walk.WalkImageUrl;
        existingWalk.DifficultyId = walk.DifficultyId;
        existingWalk.RegionId = walk.RegionId;

        await _dbContext.SaveChangesAsync();
        return existingWalk;
    }

    public async Task<Walk?> DeleteAsync(Guid id)
    {
        var existingWalk = await _dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
        if (existingWalk == null) return null;
        _dbContext.Walks.Remove(existingWalk);
        await _dbContext.SaveChangesAsync();

        return existingWalk;
    }
}
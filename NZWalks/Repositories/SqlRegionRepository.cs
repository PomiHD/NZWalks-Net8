using Microsoft.EntityFrameworkCore;
using NZWalks.Data;
using NZWalks.Models.Domain;

namespace NZWalks.Repositories;

/**
 * This class is used to get the region data from the database
 * It implements the IRegionRepository interface
 * It has a method to get all the regions
 * It returns a list of regions
 */
public class SqlRegionRepository : IRegionRepository
{
    private readonly NzWalksDbContext _dbContext;

    public SqlRegionRepository(NzWalksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Region>> GetAllAsync()
    {
        return await _dbContext.Regions.ToListAsync();
    }

    public async Task<Region?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Region> CreateAsync(Region region)
    {
        await _dbContext.Regions.AddAsync(region);
        await _dbContext.SaveChangesAsync();

        return region;
    }

    public async Task<Region?> UpdateAsync(Guid id, Region region)
    {
        var existingRegion = await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

        if (existingRegion == null)
            return null;

        existingRegion.Code = region.Code;
        existingRegion.Name = region.Name;
        existingRegion.RegionImageUrl = region.RegionImageUrl;

        await _dbContext.SaveChangesAsync();

        return existingRegion;
    }

    public async Task<Region?> DeleteAsync(Guid id)
    {
        var existingRegion = await _dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

        if (existingRegion == null)
            return null;

        _dbContext.Regions.Remove(existingRegion);
        await _dbContext.SaveChangesAsync();

        return existingRegion;
    }
}

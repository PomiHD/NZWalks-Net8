using Microsoft.EntityFrameworkCore;
using NZWalks.Data;
using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;

/**
 *  This class is used to get the region data from the database
 * It implements the IRegionRepository interface
 * It has a method to get all the regions
 * It returns a list of regions
 */
public class SqlRegionRepository:IRegionRepository

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
}
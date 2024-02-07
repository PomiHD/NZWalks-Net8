using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;
/**
 * This class is used to get the region data from memory
 * It implements the IRegionRepository interface
 * It has a method to get all the regions
 * It returns a list of regions
 */
public class InMemoryRegionRepository:IRegionRepository
{
    public async Task<List<Region>> GetAllAsync()
    {
        return new List<Region>()
        {
            new Region()
            {
                Id = Guid.NewGuid(),
                Code = "SAM",
                Name = "Southland",
            }
        };
    }
}
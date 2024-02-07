using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;

public interface IRegionRepository
{
    Task<List<Region>> GetAllAsync();
    
}
using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;

public interface IWalkRepository
{
    Task<Walk> CreateAsync(Walk walk);
}
using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;

public interface IImageRepository
{
    Task<Image> Upload(Image image);
}
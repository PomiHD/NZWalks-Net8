using NZWalks.Data;
using NZWalks.Models.Domain;

namespace NZWalks.Models.Repositories;

public class LocalImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NzWalksDbContext _dbContext;

    public LocalImageRepository(
        IWebHostEnvironment webHostEnvironment,
        IHttpContextAccessor httpContextAccessor,
        NzWalksDbContext dbContext
    )
    {
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public async Task<Image> Upload(Image image)
    {
        var localFilePath = Path.Combine(
            _webHostEnvironment.ContentRootPath,
            "Images",
            $"{image.FileName}{image.FileExtension}"
        );
        // Save the file to the local file system
        using var fileStream = new FileStream(localFilePath, FileMode.Create);
        await image.File.CopyToAsync(fileStream);

        // Create a URL to the file
        var urlFilePath =
            $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
        image.FilePath = urlFilePath;
        //add images to the Images table
        await _dbContext.Images.AddAsync(image);
        await _dbContext.SaveChangesAsync();

        return image;
    }
}

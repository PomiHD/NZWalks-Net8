using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Models.Repositories;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepositoty;

        public ImagesController(IImageRepository imageRepositoty)
        {
            _imageRepositoty = imageRepositoty;
        }

        // POST: api/Images/Upload
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);
            if (ModelState.IsValid)
            {
                //Convert dto to model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription
                };
                // user repository to upload image
                await _imageRepositoty.Upload(imageDomainModel);
                return Ok(imageDomainModel);
            }
            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(request.File.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("file", "Invalid file type");
            }
            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError(
                    "file",
                    "File size more than 10MB , Please upload a smaller file"
                );
            }
        }
    }
}

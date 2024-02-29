using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Models.DTO;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        // POST: api/Images/Upload
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            if (ModelState.IsValid)
            {
                // Validate file upload
            }
            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(request.File.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid file type");
                ModelState.AddModelError("file", "Invalid file type");
            }
            if (request.File.Length > 10485760)
            {
                throw new Exception("File size too large");
                ModelState.AddModelError(
                    "file",
                    "File size more than 10MB , Please upload a smaller file"
                );
            }
        }
    }
}
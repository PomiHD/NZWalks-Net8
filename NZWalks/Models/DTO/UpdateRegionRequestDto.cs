using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO;

public class UpdateRegionRequestDto
{
    [Required]
    [MinLength(3, ErrorMessage = "Code must be 3 characters long")]
    public string Code { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "Name must be less than 50 characters long")]
    public string Name { get; set; }

    public string? RegionImageUrl { get; set; }
}
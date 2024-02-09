using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO;

public class AddWalkRequestDto
{
    [Required]
    [MaxLength(50, ErrorMessage = "Name must be less than 50 characters long")]
    public string Name { get; set; }

    [Required]
    [MaxLength(500, ErrorMessage = "Description must be less than 500 characters long")]
    public string Description { get; set; }

    [Required]
    [Range(0.1, 100, ErrorMessage = "Length must be between 0.1 and 100 km")]
    public double LengthKm { get; set; }

    public string? WalkImageUrl { get; set; }
    [Required] public Guid DifficultyId { get; set; }
    [Required] public Guid RegionId { get; set; }
}
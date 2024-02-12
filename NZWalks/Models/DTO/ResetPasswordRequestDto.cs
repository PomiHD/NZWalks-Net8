using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO;

public class ResetPasswordRequestDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Username { get; set; }
}
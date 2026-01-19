using System.ComponentModel.DataAnnotations;

namespace Dash.Application.DTOs.Authentication;

public class RegisterRequest
{
    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
}

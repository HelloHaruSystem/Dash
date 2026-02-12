using System.ComponentModel.DataAnnotations;

namespace Dash.Application.Features.Authentication.DTOs;

public sealed record RegisterRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^\S*$", ErrorMessage = "No spaces allowed")]
    public required string Username { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public required string Password { get; init; }
}

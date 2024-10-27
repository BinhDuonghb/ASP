using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class RegisterRequest
{
    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "StudentCode must contain 10 digit.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "StudentCode All digit should be number.")]
    public string StudentCode { get; set; } = string.Empty;

    [Required] public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "PhoneNumber must contain 10 digit.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "PhoneNumber All digit should be number.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "PassWord must be alike")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
}
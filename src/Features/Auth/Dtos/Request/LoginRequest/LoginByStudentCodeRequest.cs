using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;

public class LoginByStudentCodeRequest : ILoginRequest
{
    [Required] public string StudentCode { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string Identifier => StudentCode;
}
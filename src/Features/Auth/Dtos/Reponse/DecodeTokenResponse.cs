using System.Security.Claims;

namespace HUBT_Social_API.Features.Auth.Dtos.Reponse;

public class DecodeTokenResponse
{
    public ClaimsPrincipal? ClaimsPrincipal { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Success { get; set; }
}
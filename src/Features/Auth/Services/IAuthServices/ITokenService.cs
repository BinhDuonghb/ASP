using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.IAuthServices;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(AUser user);
    DecodeTokenResponse ValidateToken(string accessToken);
    Task<TokenResponse> RefreshToken(RefreshTokenRequest request);
    Task<UserResponse> GetCurrentUser(string accessToken);
}
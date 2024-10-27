using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services;

public interface IUserManagerS
{
    Task<LoginResponse> Login(ILoginRequest request);
    Task<RegisterResponse> Register(RegisterRequest request);
    Task<string> GenerateToken(AUser user);
    Task<TokenResponse> RefreshToken(RefreshTokenRequest request);
    Task<UserResponse> GetCurrentUser(string accessToken);

    DecodeTokenResponse ValidateToken(string accessToken);
}


using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.src.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.src.Features.Auth.Services.IAuthServices
{
    public interface IAuthService
    {
        Task<(IdentityResult, AUser)> RegisterAsync(RegisterRequest model);
        Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model);
        
        Task<AUser> VerifyCodeAsync(VLpostcodeRequest request);
        Task<bool> ChangeLanguage(ChangeLanguageRequest changeLanguageRequest);
    }
}   
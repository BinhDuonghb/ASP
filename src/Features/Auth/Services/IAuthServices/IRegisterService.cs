using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.IAuthServices;

public interface IRegisterService
{
    Task<(IdentityResult, AUser)> RegisterAsync(RegisterRequest model);
}
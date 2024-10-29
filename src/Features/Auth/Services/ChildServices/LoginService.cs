using System.ComponentModel.DataAnnotations;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.IAuthServices;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Features.Auth.Services.ChildServices;

public class LoginService : ILoginService
{
    private readonly SignInManager<AUser> _signInManager;
    private readonly UserManager<AUser> _userManager;

    public LoginService(SignInManager<AUser> signInManager, UserManager<AUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<(SignInResult, AUser?)> LoginAsync(ILoginRequest model)
    {
        var user = await FindUserByIdentifierAsync(model);
        if (user == null)
            return (SignInResult.Failed, null);

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
        return result.Succeeded
            ? (result, await _userManager.FindByNameAsync(model.Identifier))
            : (result, null);
    }


    private async Task<AUser?> FindUserByIdentifierAsync(ILoginRequest identifier)
    {
        AUser? user = null;


        if (new EmailAddressAttribute().IsValid(identifier.Identifier))
        {
            user = await _userManager.FindByEmailAsync(identifier.Identifier);
        }

        else
        {
            user = await _userManager.FindByNameAsync(identifier.Identifier);
            if (!await _userManager.CheckPasswordAsync(user, identifier.Password)) return null;
        }

        return user;
    }
}
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Models;

namespace HUBT_Social_API.Features.Auth.Services.IAuthServices;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequest emailRequest);
    Task<Postcode> CreatePostcode(string reciver);
    Task<AUser> ValidatePostcode(VLpostcodeRequest postcode);
}
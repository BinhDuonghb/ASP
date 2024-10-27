using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.IAuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

[Route("EmailAuth")]
[ApiController]
[Authorize]
public class EmailAuthenticationController : ControllerBase

{
    private readonly IUserManagerS _userManagerService;

    private readonly ILogger<EmailAuthenticationController> _logger;
    private readonly IEmailService _emailService;

    public EmailAuthenticationController(IUserManagerS userManagerService,
        ILogger<EmailAuthenticationController> logger, IEmailService emailSender)
    {
        _userManagerService = userManagerService;
        _logger = logger;
        _emailService = emailSender;
    }

    [HttpPost("ValidateEmail")]
    [AllowAnonymous]
    public async Task<ActionResult> EmailLogin(string revicer)
    {
        var code = await _emailService.CreatePostcode(revicer);

        await _emailService.SendEmailAsync(new EmailRequest
            { Code = code.Code, Subject = "Validate Email Code", ToEmail = revicer });

        var result = await _userManagerService.Login(new LoginByEmailRequest { Email = revicer, Password = "" });

        return result.Success ? Ok(new { result.AccessToken }) : BadRequest(result.Message);
    }
/*        [HttpPost("ValidatePostcode")]
        public async Task<ActionResult> ValidatePostcode(string postCode)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            UserResponse result = await _userManagerService.GetCurrentUser(token);

            return await _emailService.ValidatePostcode(new VLpostcodeRequest { UserName = result.StudentCode ,Postcode = postCode }) ? Ok() : BadRequest();

        }
*/
}
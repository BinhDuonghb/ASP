using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Services.IAuthServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITokenService _tokenService;

    public AccountController(IAuthService authService, IStringLocalizer<SharedResource> localizer,
        ITokenService tokenService, IEmailService emailService = null)
    {
        _authService = authService;
        _localizer = localizer;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    // Đăng ký tài khoản mới và gửi mã OTP qua email
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new AuthResponse(
                    false,
                    400,
                    _localizer["InvalidInformation"]
                )
            );

        var (result, user) = await _authService.RegisterAsync(request);
        if (!result.Succeeded)
            return BadRequest(
                new AuthResponse(
                    false,
                    400,
                    _localizer["RegistrationFailed"],
                    result.Errors));

        // Gửi mã OTP qua email để xác thực
        try
        {
            var code = await _emailService.CreatePostcode(request.Email);

            await _emailService.SendEmailAsync(new EmailRequest
                { Code = code.Code, Subject = "Validate Email Code", ToEmail = request.Email });
        }
        catch (Exception)
        {
            return StatusCode(
                500,
                new AuthResponse(
                    false,
                    500,
                    _localizer["UnableToSendOTP"]
                )
            );
        }

        return Ok(
            new AuthResponse(
                true,
                200,
                _localizer["RegistrationSuccess"]
            )
        );
    }


    // Đăng nhập và gửi mã OTP qua email
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginByStudentCodeRequest model)
    {
        var (result, user) = await _authService.LoginAsync(model);

        if (result.Succeeded && user is not null)
        {
            try
            {
                var code = await _emailService.CreatePostcode(user.Email);

                await _emailService.SendEmailAsync(new EmailRequest
                    { Code = code.Code, Subject = "Validate Email Code", ToEmail = user.Email });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new AuthResponse(
                        false,
                        500,
                        _localizer["UnableToSendOTP"]
                    )
                );
            }

            return Ok(
                new AuthResponse(
                    true,
                    200,
                    _localizer["StepOneVerificationSuccess"]
                )
            );
        }

        if (result.IsLockedOut)
            return BadRequest(
                new AuthResponse(
                    false,
                    403,
                    _localizer["AccountLocked"]
                )
            );
        if (result.IsNotAllowed)
            return BadRequest(
                new AuthResponse(
                    false,
                    403,
                    _localizer["LoginNotAllowed"]
                )
            );
        if (result.RequiresTwoFactor)
            return BadRequest(
                new AuthResponse(
                    false,
                    401,
                    _localizer["TwoFactorRequired"]
                )
            );
        return BadRequest(
            new AuthResponse(
                false,
                400,
                _localizer["InvalidCredentials"]
            )
        );
    }

    // Xác thực mã OTP và tạo token nếu thành công
    [HttpPost("confirm-code")]
    public async Task<IActionResult> ConfirmCode([FromBody] ValidatePostcodeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new AuthResponse(
                    false,
                    400,
                    _localizer["InvalidInformation"]
                )
            );

        var user = await _authService.VerifyCodeAsync(request);
        if (user is not null)
        {
            var token = await _tokenService.GenerateTokenAsync(user);

            return Ok(
                new AuthResponse(
                    true,
                    200,
                    _localizer["VerificationSuccess"],
                    new { Token = token }
                )
            );
        }

        return Unauthorized(
            new AuthResponse(
                false,
                401,
                _localizer["OTPVerificationFailed"]
            )
        );
    }

    // Thay đổi ngôn ngữ
    [HttpPost("change-language")]
    public async Task<IActionResult> ChangeLanguage([FromBody] ChangeLanguageRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Language))
            return BadRequest(
                new AuthResponse(
                    false,
                    400,
                    _localizer["InvalidLanguage"]
                )
            );

        // Gọi phương thức trong AccountService để thay đổi ngôn ngữ
        var result = await _authService.ChangeLanguage(request);
        if (result)
            return Ok(
                new AuthResponse(
                    true,
                    200,
                    _localizer["LanguageChanged"]
                )
            );

        return BadRequest(
            new AuthResponse(
                false,
                400,
                _localizer["LanguageChangeFailed"]
            )
        );
    }
}


using HUBT_Social_API.src.Features.Auth.Dtos.Collections;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Authentication.Models;
using HUBTSOCIAL.Resources;
using HUBTSOCIAL.Src.Features.Auth.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HUBTSOCIAL.Src.Features.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(IAuthService authService, IStringLocalizer<SharedResource> localizer, ITokenService tokenService, IEmailService emailService = null)
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
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 400,
                        message: _localizer["InvalidInformation"]
                    )
                );
            }

            var (result, user) = await _authService.RegisterAsync(request);
            if (!result.Succeeded)
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 400,
                        message: _localizer["RegistrationFailed"],
                        result.Errors));
            }

            // Gửi mã OTP qua email để xác thực
            try
            {
                Postcode code = await _emailService.CreatePostcode(request.StudentCode);

                await _emailService.SendEmailAsync(new EmailRequest { Code = code.Code, Subject = "Validate Email Code", ToEmail = request.Email });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new AuthResponse(
                        success: false,
                        statusCode: 500,
                        message: _localizer["UnableToSendOTP"]
                    )
                );
            }
            return Ok(
                new AuthResponse(
                    success: true,
                    statusCode: 200,
                    message: _localizer["RegistrationSuccess"]
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
                    Postcode code = await _emailService.CreatePostcode(user.UserName);

                    await _emailService.SendEmailAsync(new EmailRequest { Code = code.Code, Subject = "Validate Email Code", ToEmail = user.Email });
                }
                catch (Exception ex)
                {
                    return StatusCode(
                        500,
                        new AuthResponse(
                            success: false,
                            statusCode: 500,
                            message: _localizer["UnableToSendOTP"]
                        )
                    );
                }

                return Ok(
                    new AuthResponse(
                        success: true,
                        statusCode: 200,
                        message: _localizer["StepOneVerificationSuccess"]
                    )
                );
            }
            else if (result.IsLockedOut)
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 403,
                        message: _localizer["AccountLocked"]
                    )
                );
            }
            else if (result.IsNotAllowed)
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 403,
                        message: _localizer["LoginNotAllowed"]
                    )
                );
            }
            else if (result.RequiresTwoFactor)
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 401,
                        message: _localizer["TwoFactorRequired"]
                    )
                );
            }
            else
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 400,
                        message: _localizer["InvalidCredentials"]
                    )
                );
            }
        }

        // Xác thực mã OTP và tạo token nếu thành công
        [HttpPost("confirm-code")]
        public async Task<IActionResult> ConfirmCode([FromBody] VLpostcodeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 400,
                        message: _localizer["InvalidInformation"]
                    )
                );
            }

            AUser user = await _authService.VerifyCodeAsync(request);
            if (user is not null)
            {
                var token = await _tokenService.GenerateTokenAsync(user);

                return Ok(
                    new AuthResponse(
                        success: true,
                        statusCode: 200,
                        message: _localizer["VerificationSuccess"],
                        data: new { Token = token }
                    )
                );
            }

            return Unauthorized(
                new AuthResponse(
                    success: false,
                    statusCode: 401,
                    message: _localizer["OTPVerificationFailed"]
                )
            );
        }

        // Thay đổi ngôn ngữ
        [HttpPost("change-language")]
        public async Task<IActionResult> ChangeLanguage([FromBody] ChangeLanguageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Language))
            {
                return BadRequest(
                    new AuthResponse(
                        success: false,
                        statusCode: 400,
                        message: _localizer["InvalidLanguage"]
                    )
                );
            }

            // Gọi phương thức trong AccountService để thay đổi ngôn ngữ
            bool result = await _authService.ChangeLanguage(request);
            if (result)
            {
                return Ok(
                    new AuthResponse(
                        success: true,
                        statusCode: 200,
                        message: _localizer["LanguageChanged"]
                    )
                );
            }

            return BadRequest(
                new AuthResponse(
                    success: false,
                    statusCode: 400,
                    message: _localizer["LanguageChangeFailed"]
                )
            );
        }
    }
}

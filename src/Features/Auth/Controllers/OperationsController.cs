using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.IAuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_API.Features.Auth.Controllers;

[Route("api/operations")]
[ApiController]
public class OperationsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OperationsController> _logger;
    private readonly RoleManager<ARole> _roleManager;
    private readonly IUserManagerS _userManagerService;

    public OperationsController(IUserManagerS userManagerService, RoleManager<ARole> roleManager,
        ILogger<OperationsController> logger, IEmailService emailSender)
    {
        _userManagerService = userManagerService;
        _roleManager = roleManager;
        _logger = logger;
        _emailService = emailSender;
    }

    [HttpPost("addRole")]
    public async Task<ActionResult> CreateRole([FromBody] CreateRoleRequest createRoleRequest)
    {
        ARole newRole = new() { Name = createRoleRequest.RoleName };
        var result = await _roleManager.CreateAsync(newRole);
        if (!result.Succeeded)
        {
            _logger.LogError(result.Errors.First().Description);
            return BadRequest(new { Message = $"Create role fail  {result?.Errors?.First()?.Description}" });
        }

        _logger.LogInformation("Crate role.");
        return Ok(new { Message = "Create role success" });
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register(RegisterRequest registerRequest)
    {
        var result = await _userManagerService.Register(registerRequest);
        _logger.LogInformation("User has register.");
        return result.Success ? Ok(result.Message) : BadRequest(result.Message);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login(LoginByStudentCodeRequest loginRequest)
    {
        var result = await _userManagerService.Login(loginRequest);
        _logger.LogInformation("User logged in.");
        return result.Success ? Ok(new { result.AccessToken }) : BadRequest(result.Message);
    }

    [HttpPut]
    [Route("refreshToken")]
    public async Task<ActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _userManagerService.RefreshToken(request);
        _logger.LogInformation("User Token is refreshed.");
        return result.Success ? Ok(result.Token) : Unauthorized(result.Message);
    }

    [HttpGet("ValidateToken")]
    public ActionResult ValidateToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var result = _userManagerService.ValidateToken(token);
        return result.Success ? Ok() : Unauthorized(result.Message);
    }
}
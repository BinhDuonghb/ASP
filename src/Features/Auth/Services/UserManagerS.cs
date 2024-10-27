using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Dtos.Request;
using HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Auth.Services;

public class UserManagerS : IUserManagerS
{
    private readonly JwtSetting _jwtSetting;
    private readonly IMongoCollection<RefreshToken> _refreshToken;
    private readonly UserManager<AUser> _userManager;

    public UserManagerS(UserManager<AUser> userManager, IOptions<JwtSetting> jwtSettingl,
        IMongoCollection<RefreshToken> refreshToken)
    {
        _userManager = userManager;
        _jwtSetting = jwtSettingl.Value;
        _refreshToken = refreshToken;
    }

    public async Task<LoginResponse> Login(ILoginRequest request)
    {
        try
        {
            var user = await FindUserByIdentifierAsync(request);
            if (user == null)
                return new LoginResponse
                {
                    Message = "Invalid StudentCode or Email/password",
                    Success = false
                };
            var token = await GenerateToken(user);
            await ReGenerateToken(user);
            return new LoginResponse
            {
                AccessToken = token,
                Success = true,
                Message = "Login Success full"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new LoginResponse
            {
                Message = e.Message,
                Success = false
            };
        }
    }


    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
                return new RegisterResponse
                {
                    Message = "Email has Already register",
                    Success = false
                };
            user = new AUser
            {
                Email = request.Email,
                UserName = request.StudentCode,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };
            var created = await _userManager.CreateAsync(user, request.Password);
            if (!created.Succeeded)
                return new RegisterResponse
                {
                    Message = $"Create user failed {created?.Errors?.First()?.Description}",
                    Success = false
                };

            var roleAdd = await _userManager.AddToRoleAsync(user, "USER");
            if (!roleAdd.Succeeded)
                return new RegisterResponse
                {
                    Message = $"Create user success but role not avaliable {roleAdd?.Errors?.First()?.Description}",
                    Success = false
                };

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };


            var setCaim = await _userManager.AddClaimsAsync(user, claims);
            if (!setCaim.Succeeded)
                return new RegisterResponse
                {
                    Message = $"Create user success but Claims not avaliable {roleAdd?.Errors?.First()?.Description}",
                    Success = false
                };
            return new RegisterResponse
            {
                Message = "Register Successfull",
                Success = true
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new RegisterResponse
            {
                Message = e.Message,
                Success = false
            };
        }
    }

    public async Task<string> GenerateToken(AUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);

        List<Claim> claims = new();
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var role = await _userManager.GetRolesAsync(user);
        var roelClaims = role.Select(x => new Claim(ClaimTypes.Role, x));
        claims.AddRange(roelClaims);


        var key = new SymmetricSecurityKey(tokenKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(30);

        var tokenDesc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = creds
        };
        var token = tokenHandler.CreateToken(tokenDesc);

        var existion = await _refreshToken.Find(t => t.UserId == user.Id.ToString()).FirstOrDefaultAsync();
        if (existion == null)
        {
            await _refreshToken.InsertOneAsync(new RefreshToken
                { AccessToken = tokenHandler.WriteToken(token), UserId = user.Id.ToString() });
        }
        else
        {
            var update = Builders<RefreshToken>.Update.Set(t => t.AccessToken, tokenHandler.WriteToken(token));
            await _refreshToken.UpdateOneAsync(t => t.UserId == existion.UserId, update);
        }

        ;


        return tokenHandler.WriteToken(token);
    }

    public async Task<TokenResponse> RefreshToken(RefreshTokenRequest request)
    {
        var refreshToken =
            await _refreshToken.Find(item => item.RefreshTo == request.RefreshToken).FirstOrDefaultAsync();
        if (refreshToken != null)
        {
            var principal = ValidateToken(refreshToken.AccessToken);

            if (!principal.Success) return new TokenResponse { Success = false, Message = principal.Message };
            var userId = principal.ClaimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return new TokenResponse { Success = false, Message = "Can't not found userId in jwt" };
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new TokenResponse { Success = false, Message = $"Can't find the user with Id of {userId}" };
            var _exsitdata = await _refreshToken.Find(t => t.UserId == userId && t.RefreshTo == request.RefreshToken)
                .FirstOrDefaultAsync();
            if (_exsitdata != null)
            {
                var finalToken = await GenerateToken(user);
                return new TokenResponse
                {
                    Token = finalToken,
                    Refresh = await ReGenerateToken(user),
                    Success = true
                };
            }

            return new TokenResponse { Success = false, Message = "Can't find the owner of this token" };
        }

        return new TokenResponse { Success = false, Message = "Refresh Token Not Found" };
    }

    public async Task<UserResponse> GetCurrentUser(string accessToken)
    {
        var decodeValue = ValidateToken(accessToken);
        if (!decodeValue.Success)
            return new UserResponse { Success = false, Message = decodeValue.Message };

        var userIdClaim = decodeValue.ClaimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return new UserResponse { Success = false, Message = "Can't find the owner of this token" };

        var user = await _userManager.FindByIdAsync(userIdClaim);
        if (user == null)
            return new UserResponse { Success = false, Message = "Can't find the owner of this token" };
        var nameParts = user.FullName.Split(' ');

        return new UserResponse
        {
            Email = user.Email, StudentCode = user.UserName, LastName = nameParts[0],
            FirstName = string.Join(" ", nameParts.Skip(1)), Success = true
        };
    }

    public DecodeTokenResponse ValidateToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey)
            }, out var securityToken);

            if (securityToken is JwtSecurityToken token && token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                if (token.ValidTo < DateTime.UtcNow)
                    return new DecodeTokenResponse { Success = false, Message = "Token is expired" };
                return new DecodeTokenResponse { Success = true, ClaimsPrincipal = principal };
            }

            return new DecodeTokenResponse { Success = false, Message = "Token is not match our Algorithms" };
        }
        catch (Exception ex)
        {
            return new DecodeTokenResponse { Success = false, Message = ex.Message };
        }
    }

    private async Task<string> ReGenerateToken(AUser user)
    {
        var randomnumber = new byte[32];
        using var randomnumbergenerate = RandomNumberGenerator.Create();

        randomnumbergenerate.GetBytes(randomnumber);
        var refreshtoken = Convert.ToBase64String(randomnumber);
        var existion = _refreshToken.Find(t => t.UserId == user.Id.ToString()).FirstOrDefaultAsync();
        if (existion != null)
        {
            var update = Builders<RefreshToken>.Update.Set(t => t.RefreshTo, refreshtoken);
            await _refreshToken.UpdateOneAsync(t => t.UserId == user.Id.ToString(), update);
        }
        else
        {
            await _refreshToken.InsertOneAsync(new RefreshToken
            {
                UserId = user.Id.ToString(),
                AccessToken = await GenerateToken(user),
                RefreshTo = refreshtoken
            });
        }

        return refreshtoken;
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
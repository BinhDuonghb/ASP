namespace HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;

public interface ILoginRequest
{
    string Identifier { get; }
    string Password { get; }
}
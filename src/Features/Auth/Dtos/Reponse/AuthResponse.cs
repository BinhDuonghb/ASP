namespace HUBT_Social_API.Features.Auth.Dtos.Reponse;

public class AuthResponse(bool success, int statusCode, string message, object? data = null)
{
    public bool Success { get; set; } = success;
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
    public object? Data { get; set; } = data;
}
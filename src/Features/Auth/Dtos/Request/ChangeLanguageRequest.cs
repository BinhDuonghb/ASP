namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class ChangeLanguageRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
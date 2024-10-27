namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class EmailRequest
{
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
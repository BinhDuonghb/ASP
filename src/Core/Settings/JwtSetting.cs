namespace HUBT_Social_API.Core.Settings;

public class JwtSetting
{
    public string SecretKey { get; set; } = string.Empty;
    public string RefreshSecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int TokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
}
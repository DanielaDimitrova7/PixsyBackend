namespace PixsyAPI.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = "Pixsy_Development_Key_12345678901234567890";
    public string Issuer { get; set; } = "PixsyAPI";
    public string Audience { get; set; } = "PixsyFrontend";
    public int ExpiresMinutes { get; set; } = 10080;
}

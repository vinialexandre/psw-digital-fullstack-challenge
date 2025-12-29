using System.ComponentModel.DataAnnotations;

namespace HolidaysAPI.Domain.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "JWT Key is required")]
    [MinLength(32, ErrorMessage = "JWT Key must be at least 32 characters for security")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT Issuer is required")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT Audience is required")]
    public string Audience { get; set; } = string.Empty;

    public int ExpirationHours { get; set; } = 24;
}


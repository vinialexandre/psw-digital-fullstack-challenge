using System.ComponentModel.DataAnnotations;

namespace HolidaysAPI.Domain.Configuration;

public class AuthSettings
{
    public const string SectionName = "Auth";

    [Required(ErrorMessage = "Admin username is required")]
    public string AdminUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "Admin password is required")]
    public string AdminPassword { get; set; } = string.Empty;
}


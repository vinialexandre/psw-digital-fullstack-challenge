namespace HolidaysAPI.Domain.Configuration;

public class AuthSettings
{
    public const string SectionName = "Auth";

    public string AdminUsername { get; set; } = string.Empty;

    public string AdminPassword { get; set; } = string.Empty;
}


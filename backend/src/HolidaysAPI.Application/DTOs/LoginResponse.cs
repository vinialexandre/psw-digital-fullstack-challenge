using System.Text.Json.Serialization;

namespace HolidaysAPI.Application.DTOs;

public class LoginResponse
{
    [JsonIgnore]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = string.Empty;
}


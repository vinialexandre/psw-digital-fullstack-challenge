using System.ComponentModel.DataAnnotations;

namespace HolidaysAPI.Application.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password é obrigatório")]
    public string Password { get; set; } = string.Empty;
}


using System.ComponentModel.DataAnnotations;

namespace HolidaysAPI.Application.DTOs;

public class HolidayDto
{
    [Required]
    public string Date { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;
}


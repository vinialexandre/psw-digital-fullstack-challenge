using HolidaysAPI.Domain.Enums;

namespace HolidaysAPI.Application.DTOs;

public class HolidayDto
{
    public string Date { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}


using HolidaysAPI.Domain.Enums;

namespace HolidaysAPI.Domain.Entities;

public class Holiday
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public HolidayType Type { get; set; }
}


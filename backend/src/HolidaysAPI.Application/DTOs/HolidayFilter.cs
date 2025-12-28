namespace HolidaysAPI.Application.DTOs;

public class HolidayFilter
{
    public int? Year { get; set; }
    public DateTime? Date { get; set; }
    public string? Type { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}


using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Enums;
using HolidaysAPI.Domain.Interfaces;

namespace HolidaysAPI.Application.Services;

public class HolidayService : IHolidayService
{
    private readonly IHolidayRepository _repository;
    private readonly ICacheService _cacheService;

    public HolidayService(IHolidayRepository repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<IEnumerable<HolidayDto>>> GetHolidaysAsync(HolidayFilter filter)
    {
        try
        {
            var year = filter.Year ?? DateTime.Now.Year;
            var cacheKey = $"holidays_{year}";

            var holidays = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () => await _repository.GetByYearAsync(year),
                TimeSpan.FromHours(24)
            );

            if (holidays == null || !holidays.Any())
            {
                return ApiResponse<IEnumerable<HolidayDto>>.ErrorResponse("No holidays found");
            }

            var filteredHolidays = ApplyFilters(holidays, filter);
            var sortedHolidays = ApplySorting(filteredHolidays, filter);
            var dtos = MapToDto(sortedHolidays);

            return ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(
                dtos,
                dtos.Count(),
                "Holidays retrieved successfully"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<HolidayDto>>.ErrorResponse($"Error retrieving holidays: {ex.Message}");
        }
    }

    private IEnumerable<Holiday> ApplyFilters(IEnumerable<Holiday> holidays, HolidayFilter filter)
    {
        if (filter.Date.HasValue)
        {
            holidays = holidays.Where(h => h.Date.Date == filter.Date.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(filter.Type))
        {
            if (Enum.TryParse<HolidayType>(filter.Type, true, out var type))
            {
                holidays = holidays.Where(h => h.Type == type);
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            holidays = holidays.Where(h => h.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return holidays;
    }

    private IEnumerable<Holiday> ApplySorting(IEnumerable<Holiday> holidays, HolidayFilter filter)
    {
        if (string.IsNullOrWhiteSpace(filter.SortBy))
        {
            return holidays.OrderBy(h => h.Date);
        }

        return filter.SortBy.ToLower() switch
        {
            "date" => filter.SortDescending ? holidays.OrderByDescending(h => h.Date) : holidays.OrderBy(h => h.Date),
            "name" => filter.SortDescending ? holidays.OrderByDescending(h => h.Name) : holidays.OrderBy(h => h.Name),
            "type" => filter.SortDescending ? holidays.OrderByDescending(h => h.Type) : holidays.OrderBy(h => h.Type),
            _ => holidays.OrderBy(h => h.Date)
        };
    }

    private IEnumerable<HolidayDto> MapToDto(IEnumerable<Holiday> holidays)
    {
        return holidays.Select(h => new HolidayDto
        {
            Date = h.Date.ToString("dd/MM/yyyy"),
            Name = h.Name,
            Type = h.Type.ToString()
        });
    }
}


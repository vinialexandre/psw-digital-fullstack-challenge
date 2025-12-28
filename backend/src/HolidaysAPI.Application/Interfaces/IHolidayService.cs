using HolidaysAPI.Application.DTOs;

namespace HolidaysAPI.Application.Interfaces;

public interface IHolidayService
{
    Task<ApiResponse<IEnumerable<HolidayDto>>> GetHolidaysAsync(HolidayFilter filter);
}


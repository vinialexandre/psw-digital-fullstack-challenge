using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysAPI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/holidays")]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService _holidayService;

    public HolidaysController(IHolidayService holidayService)
    {
        _holidayService = holidayService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<HolidayDto>>>> GetHolidays(
        [FromQuery] int? year,
        [FromQuery] DateTime? date,
        [FromQuery] string? type,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = false)
    {
        var filter = new HolidayFilter
        {
            Year = year,
            Date = date,
            Type = type,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _holidayService.GetHolidaysAsync(filter);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}


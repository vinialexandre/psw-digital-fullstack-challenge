using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Interfaces;
using HolidaysAPI.Infrastructure.ExternalServices;

namespace HolidaysAPI.Infrastructure.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly IBrasilApiService _brasilApiService;

    public HolidayRepository(IBrasilApiService brasilApiService)
    {
        _brasilApiService = brasilApiService;
    }

    public async Task<IEnumerable<Holiday>> GetAllAsync()
    {
        return await GetByYearAsync(DateTime.Now.Year);
    }

    public async Task<IEnumerable<Holiday>> GetByYearAsync(int year)
    {
        return await _brasilApiService.GetHolidaysAsync(year);
    }
}


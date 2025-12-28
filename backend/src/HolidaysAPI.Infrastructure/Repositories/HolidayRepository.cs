using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Interfaces;
using HolidaysAPI.Infrastructure.ExternalServices;

namespace HolidaysAPI.Infrastructure.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly BrasilApiService _brasilApiService;

    public HolidayRepository(BrasilApiService brasilApiService)
    {
        _brasilApiService = brasilApiService;
    }

    public async Task<IEnumerable<Holiday>> GetAllAsync()
    {
        return await _brasilApiService.GetHolidaysAsync(2025);
    }
}


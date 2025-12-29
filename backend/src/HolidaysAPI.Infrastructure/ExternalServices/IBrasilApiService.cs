using HolidaysAPI.Domain.Entities;

namespace HolidaysAPI.Infrastructure.ExternalServices;

public interface IBrasilApiService
{
    Task<IEnumerable<Holiday>> GetHolidaysAsync(int year);
}


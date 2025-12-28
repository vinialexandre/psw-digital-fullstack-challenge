using HolidaysAPI.Domain.Entities;

namespace HolidaysAPI.Domain.Interfaces;

public interface IHolidayRepository : IRepository<Holiday>
{
    Task<IEnumerable<Holiday>> GetByYearAsync(int year);
}


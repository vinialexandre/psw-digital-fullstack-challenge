using System.Text.Json;
using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Enums;

namespace HolidaysAPI.Infrastructure.ExternalServices;

public class BrasilApiService
{
    private readonly HttpClient _httpClient;

    public BrasilApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(int year)
    {
        var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/feriados/v1/{year}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var brasilApiHolidays = JsonSerializer.Deserialize<List<BrasilApiHoliday>>(content);

        if (brasilApiHolidays == null)
        {
            return Enumerable.Empty<Holiday>();
        }

        return brasilApiHolidays.Select(h => new Holiday
        {
            Date = DateTime.Parse(h.date),
            Name = h.name,
            Type = h.type == "national" ? HolidayType.National : HolidayType.Municipal
        });
    }

    private class BrasilApiHoliday
    {
        public string date { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
    }
}


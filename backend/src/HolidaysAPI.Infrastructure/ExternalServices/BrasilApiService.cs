using System.Text.Json;
using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Enums;

namespace HolidaysAPI.Infrastructure.ExternalServices;

public class BrasilApiService : IBrasilApiService
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

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var brasilApiHolidays = JsonSerializer.Deserialize<List<BrasilApiHoliday>>(content, options);

        if (brasilApiHolidays == null)
        {
            return Enumerable.Empty<Holiday>();
        }

        return brasilApiHolidays.Select(h => new Holiday
        {
            Date = DateTime.Parse(h.Date),
            Name = h.Name,
            Type = h.Type.Equals("national", StringComparison.OrdinalIgnoreCase)
                ? HolidayType.National
                : HolidayType.Municipal
        });
    }

    private class BrasilApiHoliday
    {
        public string Date { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}


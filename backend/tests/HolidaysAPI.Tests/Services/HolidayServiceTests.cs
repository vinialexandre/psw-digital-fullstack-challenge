using FluentAssertions;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using HolidaysAPI.Application.Services;
using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Enums;
using HolidaysAPI.Domain.Interfaces;
using Moq;

namespace HolidaysAPI.Tests.Services;

public class HolidayServiceTests
{
    private readonly Mock<IHolidayRepository> _mockRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly HolidayService _service;

    public HolidayServiceTests()
    {
        _mockRepository = new Mock<IHolidayRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _service = new HolidayService(_mockRepository.Object, _mockCacheService.Object);
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldReturnAllHolidays_WhenNoFilterApplied()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 1, 1), Name = "New Year", Type = HolidayType.National },
            new() { Date = new DateTime(2025, 12, 25), Name = "Christmas", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter();
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldFilterByType_WhenTypeProvided()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 1, 1), Name = "New Year", Type = HolidayType.National },
            new() { Date = new DateTime(2025, 6, 12), Name = "City Day", Type = HolidayType.Municipal }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter { Type = "National" };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("New Year");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldFilterBySearchTerm_WhenSearchTermProvided()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 1, 1), Name = "New Year", Type = HolidayType.National },
            new() { Date = new DateTime(2025, 12, 25), Name = "Christmas", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter { SearchTerm = "Christmas" };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("Christmas");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldSortByDate_WhenSortByDateProvided()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 12, 25), Name = "Christmas", Type = HolidayType.National },
            new() { Date = new DateTime(2025, 1, 1), Name = "New Year", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter { SortBy = "date", SortDescending = false };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Name.Should().Be("New Year");
        result.Data!.Last().Name.Should().Be("Christmas");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldReturnFormattedDate_InBrazilianFormat()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 1, 1), Name = "New Year", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter();
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Date.Should().Be("01/01/2025");
    }
}


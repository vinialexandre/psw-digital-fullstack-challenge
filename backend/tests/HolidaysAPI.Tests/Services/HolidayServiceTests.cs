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

    [Fact]
    public async Task GetHolidaysAsync_WhenNoHolidaysFound_ReturnsErrorResponse()
    {
        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(Enumerable.Empty<Holiday>());

        var filter = new HolidayFilter();
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("No holidays found");
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenNullReturned_ReturnsErrorResponse()
    {
        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync((IEnumerable<Holiday>?)null);

        var filter = new HolidayFilter();
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("No holidays found");
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenExceptionThrown_ReturnsErrorResponse()
    {
        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ThrowsAsync(new Exception("Database error"));

        var filter = new HolidayFilter();
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Error retrieving holidays");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldFilterByDate_WhenDateProvided()
    {
        var targetDate = new DateTime(2025, 1, 1);
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

        var filter = new HolidayFilter { Date = targetDate };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("New Year");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldSortByName_WhenSortByNameProvided()
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

        var filter = new HolidayFilter { SortBy = "name", SortDescending = false };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Name.Should().Be("Christmas");
        result.Data!.Last().Name.Should().Be("New Year");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldSortByType_WhenSortByTypeProvided()
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

        var filter = new HolidayFilter { SortBy = "type", SortDescending = false };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldSortDescending_WhenSortDescendingTrue()
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

        var filter = new HolidayFilter { SortBy = "date", SortDescending = true };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Name.Should().Be("Christmas");
        result.Data!.Last().Name.Should().Be("New Year");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldUseDefaultSort_WhenInvalidSortByProvided()
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

        var filter = new HolidayFilter { SortBy = "invalid" };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Name.Should().Be("New Year");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldUseSpecificYear_WhenYearProvided()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2024, 1, 1), Name = "New Year 2024", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                "holidays_2024",
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter { Year = 2024 };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Name.Should().Be("New Year 2024");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldFilterByInvalidType_ReturnsAllHolidays()
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

        var filter = new HolidayFilter { Type = "InvalidType" };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldSortByNameDescending_WhenRequested()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 1, 1), Name = "Alpha", Type = HolidayType.National },
            new() { Date = new DateTime(2025, 12, 25), Name = "Zeta", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter { SortBy = "name", SortDescending = true };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data!.First().Name.Should().Be("Zeta");
        result.Data!.Last().Name.Should().Be("Alpha");
    }

    [Fact]
    public async Task GetHolidaysAsync_ShouldSortByTypeDescending_WhenRequested()
    {
        var holidays = new List<Holiday>
        {
            new() { Date = new DateTime(2025, 6, 12), Name = "City Day", Type = HolidayType.Municipal },
            new() { Date = new DateTime(2025, 1, 1), Name = "New Year", Type = HolidayType.National }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<Holiday>>>>(),
                It.IsAny<TimeSpan?>()))
            .ReturnsAsync(holidays);

        var filter = new HolidayFilter { SortBy = "type", SortDescending = true };
        var result = await _service.GetHolidaysAsync(filter);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }
}


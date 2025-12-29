using FluentAssertions;
using HolidaysAPI.Domain.Entities;
using HolidaysAPI.Domain.Enums;
using HolidaysAPI.Infrastructure.ExternalServices;
using HolidaysAPI.Infrastructure.Repositories;
using Moq;

namespace HolidaysAPI.Tests.Infrastructure;

public class HolidayRepositoryTests
{
    private readonly Mock<IBrasilApiService> _brasilApiServiceMock;
    private readonly HolidayRepository _repository;

    public HolidayRepositoryTests()
    {
        _brasilApiServiceMock = new Mock<IBrasilApiService>();
        _repository = new HolidayRepository(_brasilApiServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsHolidaysForCurrentYear()
    {
        var currentYear = DateTime.Now.Year;
        var expectedHolidays = new List<Holiday>
        {
            new() { Date = new DateTime(currentYear, 1, 1), Name = "Ano Novo", Type = HolidayType.National }
        };

        _brasilApiServiceMock.Setup(x => x.GetHolidaysAsync(currentYear))
            .ReturnsAsync(expectedHolidays);

        var result = await _repository.GetAllAsync();

        result.Should().BeEquivalentTo(expectedHolidays);
    }

    [Fact]
    public async Task GetByYearAsync_ReturnsHolidaysForSpecifiedYear()
    {
        var year = 2024;
        var expectedHolidays = new List<Holiday>
        {
            new() { Date = new DateTime(year, 1, 1), Name = "Ano Novo", Type = HolidayType.National },
            new() { Date = new DateTime(year, 12, 25), Name = "Natal", Type = HolidayType.National }
        };

        _brasilApiServiceMock.Setup(x => x.GetHolidaysAsync(year))
            .ReturnsAsync(expectedHolidays);

        var result = await _repository.GetByYearAsync(year);

        result.Should().BeEquivalentTo(expectedHolidays);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByYearAsync_WhenNoHolidays_ReturnsEmptyList()
    {
        var year = 2024;
        var expectedHolidays = new List<Holiday>();

        _brasilApiServiceMock.Setup(x => x.GetHolidaysAsync(year))
            .ReturnsAsync(expectedHolidays);

        var result = await _repository.GetByYearAsync(year);

        result.Should().BeEmpty();
    }
}


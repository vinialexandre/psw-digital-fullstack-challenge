using FluentAssertions;
using HolidaysAPI.API.Controllers;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HolidaysAPI.Tests.Controllers;

public class HolidaysControllerTests
{
    private readonly Mock<IHolidayService> _holidayServiceMock;
    private readonly HolidaysController _controller;

    public HolidaysControllerTests()
    {
        _holidayServiceMock = new Mock<IHolidayService>();
        _controller = new HolidaysController(_holidayServiceMock.Object);
    }

    [Fact]
    public async Task GetHolidays_WithoutFilters_ReturnsAllHolidays()
    {
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-01-01", Name = "Ano Novo", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.IsAny<HolidayFilter>()))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, null, null, null, null, false);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(apiResponse);
    }

    [Fact]
    public async Task GetHolidays_WithYearFilter_ReturnsFilteredHolidays()
    {
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-01-01", Name = "Ano Novo", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.Is<HolidayFilter>(f => f.Year == 2024)))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(2024, null, null, null, null, false);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHolidays_WithDateFilter_ReturnsFilteredHolidays()
    {
        var date = new DateTime(2024, 1, 1);
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-01-01", Name = "Ano Novo", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.Is<HolidayFilter>(f => f.Date == date)))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, date, null, null, null, false);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHolidays_WithTypeFilter_ReturnsFilteredHolidays()
    {
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-01-01", Name = "Ano Novo", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.Is<HolidayFilter>(f => f.Type == "national")))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, null, "national", null, null, false);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHolidays_WithSearchTerm_ReturnsFilteredHolidays()
    {
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-01-01", Name = "Ano Novo", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.Is<HolidayFilter>(f => f.SearchTerm == "Ano")))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, null, null, "Ano", null, false);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHolidays_WithSortBy_ReturnsSortedHolidays()
    {
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-01-01", Name = "Ano Novo", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.Is<HolidayFilter>(f => f.SortBy == "name")))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, null, null, null, "name", false);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHolidays_WithSortDescending_ReturnsSortedDescending()
    {
        var holidays = new List<HolidayDto>
        {
            new() { Date = "2024-12-25", Name = "Natal", Type = "national" }
        };
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.SuccessResponse(holidays);

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.Is<HolidayFilter>(f => f.SortDescending == true)))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, null, null, null, null, true);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHolidays_WhenServiceFails_ReturnsBadRequest()
    {
        var apiResponse = ApiResponse<IEnumerable<HolidayDto>>.ErrorResponse("Service error");

        _holidayServiceMock.Setup(x => x.GetHolidaysAsync(It.IsAny<HolidayFilter>()))
            .ReturnsAsync(apiResponse);

        var result = await _controller.GetHolidays(null, null, null, null, null, false);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}


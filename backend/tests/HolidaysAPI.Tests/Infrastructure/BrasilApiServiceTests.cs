using System.Net;
using System.Text.Json;
using FluentAssertions;
using HolidaysAPI.Domain.Enums;
using HolidaysAPI.Infrastructure.ExternalServices;
using Moq;
using Moq.Protected;

namespace HolidaysAPI.Tests.Infrastructure;

public class BrasilApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly BrasilApiService _service;

    public BrasilApiServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _service = new BrasilApiService(_httpClient);
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenApiReturnsHolidays_ReturnsHolidayList()
    {
        var year = 2025;
        var apiResponse = new[]
        {
            new { date = "2025-01-01", name = "Confraternizacao Universal", type = "national" },
            new { date = "2025-12-25", name = "Natal", type = "national" }
        };
        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        var result = await _service.GetHolidaysAsync(year);

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Confraternizacao Universal");
        result.First().Type.Should().Be(HolidayType.National);
        result.First().Date.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenApiReturnsMunicipalHoliday_ReturnsWithMunicipalType()
    {
        var year = 2025;
        var apiResponse = new[]
        {
            new { date = "2025-01-25", name = "Aniversario de Sao Paulo", type = "municipal" }
        };
        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        var result = await _service.GetHolidaysAsync(year);

        result.Should().HaveCount(1);
        result.First().Type.Should().Be(HolidayType.Municipal);
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenApiReturnsEmptyArray_ReturnsEmptyList()
    {
        var year = 2025;
        var jsonResponse = "[]";

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        var result = await _service.GetHolidaysAsync(year);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenApiReturnsNull_ReturnsEmptyList()
    {
        var year = 2025;
        var jsonResponse = "null";

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        var result = await _service.GetHolidaysAsync(year);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenApiReturnsError_ThrowsException()
    {
        var year = 2025;

        SetupHttpResponse(HttpStatusCode.InternalServerError, "Internal Server Error");

        Func<Task> act = async () => await _service.GetHolidaysAsync(year);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetHolidaysAsync_WhenApiReturnsNotFound_ThrowsException()
    {
        var year = 1899;

        SetupHttpResponse(HttpStatusCode.NotFound, "Not Found");

        Func<Task> act = async () => await _service.GetHolidaysAsync(year);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetHolidaysAsync_CorrectlyParsesMultipleHolidayTypes()
    {
        var year = 2025;
        var apiResponse = new[]
        {
            new { date = "2025-01-01", name = "Ano Novo", type = "national" },
            new { date = "2025-01-25", name = "Aniversario SP", type = "municipal" },
            new { date = "2025-04-21", name = "Tiradentes", type = "NATIONAL" }
        };
        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        var result = await _service.GetHolidaysAsync(year);
        var holidays = result.ToList();

        holidays.Should().HaveCount(3);
        holidays[0].Type.Should().Be(HolidayType.National);
        holidays[1].Type.Should().Be(HolidayType.Municipal);
        holidays[2].Type.Should().Be(HolidayType.National);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }
}


using FluentAssertions;
using HolidaysAPI.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HolidaysAPI.Tests.Controllers;

public class HealthControllerTests
{
    private readonly Mock<HealthCheckService> _healthCheckServiceMock;
    private readonly Mock<ILogger<HealthController>> _loggerMock;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _healthCheckServiceMock = new Mock<HealthCheckService>();
        _loggerMock = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_healthCheckServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Get_WhenHealthy_ReturnsOk()
    {
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["self"] = new HealthReportEntry(HealthStatus.Healthy, null, TimeSpan.Zero, null, null)
            },
            TimeSpan.FromMilliseconds(100));

        _healthCheckServiceMock.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        var result = await _controller.Get();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task Get_WhenUnhealthy_ReturnsServiceUnavailable()
    {
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["redis"] = new HealthReportEntry(HealthStatus.Unhealthy, null, TimeSpan.Zero, new Exception("Connection failed"), null)
            },
            TimeSpan.FromMilliseconds(100));

        _healthCheckServiceMock.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        var result = await _controller.Get();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
    }

    [Fact]
    public async Task Get_WhenDegraded_ReturnsOk()
    {
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["self"] = new HealthReportEntry(HealthStatus.Degraded, null, TimeSpan.Zero, null, null)
            },
            TimeSpan.FromMilliseconds(100));

        _healthCheckServiceMock.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        var result = await _controller.Get();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task Get_WhenExceptionThrown_ReturnsInternalServerError()
    {
        _healthCheckServiceMock.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Health check failed"));

        var result = await _controller.Get();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Ready_WhenHealthy_ReturnsOk()
    {
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["self"] = new HealthReportEntry(HealthStatus.Healthy, null, TimeSpan.Zero, null, null)
            },
            TimeSpan.FromMilliseconds(100));

        _healthCheckServiceMock.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        var result = await _controller.Ready();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Ready_WhenUnhealthy_ReturnsServiceUnavailable()
    {
        var healthReport = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["redis"] = new HealthReportEntry(HealthStatus.Unhealthy, null, TimeSpan.Zero, null, null)
            },
            TimeSpan.FromMilliseconds(100));

        _healthCheckServiceMock.Setup(x => x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);

        var result = await _controller.Ready();

        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
    }

    [Fact]
    public void Live_ReturnsOk()
    {
        var result = _controller.Live();

        result.Should().BeOfType<OkObjectResult>();
    }
}


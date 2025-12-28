using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HolidaysAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        HealthCheckService healthCheckService,
        ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                status = healthReport.Status.ToString(),
                totalDuration = healthReport.TotalDuration.TotalMilliseconds,
                checks = healthReport.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration.TotalMilliseconds,
                    exception = entry.Value.Exception?.Message,
                    data = entry.Value.Data
                })
            };

            var statusCode = healthReport.Status switch
            {
                HealthStatus.Healthy => StatusCodes.Status200OK,
                HealthStatus.Degraded => StatusCodes.Status200OK,
                HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status500InternalServerError
            };

            return StatusCode(statusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar health checks");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "Unhealthy",
                error = "Erro ao verificar status da aplicacao"
            });
        }
    }

    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();
        
        if (healthReport.Status == HealthStatus.Healthy)
        {
            return Ok(new { status = "Ready" });
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new
        {
            status = "Not Ready",
            reason = healthReport.Entries
                .Where(e => e.Value.Status != HealthStatus.Healthy)
                .Select(e => e.Key)
        });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new
        {
            status = "Alive",
            timestamp = DateTime.UtcNow
        });
    }
}


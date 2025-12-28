using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysAPI.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _environment;

    public AuthController(IAuthService authService, IWebHostEnvironment environment)
    {
        _authService = authService;
        _environment = environment;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success || result.Data == null)
        {
            return Unauthorized(result);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = result.Data.ExpiresAt
        };

        Response.Cookies.Append("jwt", result.Data.Token, cookieOptions);

        return Ok(result);
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { success = true, message = "Logout successful" });
    }
}


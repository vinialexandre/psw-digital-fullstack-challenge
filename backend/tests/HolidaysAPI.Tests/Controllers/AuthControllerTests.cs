using FluentAssertions;
using HolidaysAPI.API.Controllers;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HolidaysAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _controller = new AuthController(_authServiceMock.Object, _environmentMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var loginRequest = new LoginRequest { Username = "test", Password = "test123" };
        var loginResponse = new LoginResponse
        {
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        var apiResponse = ApiResponse<LoginResponse>.SuccessResponse(loginResponse);

        _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
            .ReturnsAsync(apiResponse);
        _environmentMock.Setup(x => x.EnvironmentName).Returns("Development");

        var result = await _controller.Login(loginRequest);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(apiResponse);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest { Username = "invalid", Password = "wrong" };
        var apiResponse = ApiResponse<LoginResponse>.ErrorResponse("Invalid credentials");

        _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
            .ReturnsAsync(apiResponse);

        var result = await _controller.Login(loginRequest);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithNullData_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest { Username = "test", Password = "test" };
        var apiResponse = new ApiResponse<LoginResponse>
        {
            Success = true,
            Data = null
        };

        _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
            .ReturnsAsync(apiResponse);

        var result = await _controller.Login(loginRequest);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_InProduction_SetsCookieWithSecureFlag()
    {
        var loginRequest = new LoginRequest { Username = "test", Password = "test123" };
        var loginResponse = new LoginResponse
        {
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        var apiResponse = ApiResponse<LoginResponse>.SuccessResponse(loginResponse);

        _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
            .ReturnsAsync(apiResponse);
        _environmentMock.Setup(x => x.EnvironmentName).Returns("Production");

        await _controller.Login(loginRequest);

        var cookies = _controller.Response.Headers["Set-Cookie"];
        cookies.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_InDevelopment_SetsCookieWithLaxSameSite()
    {
        var loginRequest = new LoginRequest { Username = "test", Password = "test123" };
        var loginResponse = new LoginResponse
        {
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        var apiResponse = ApiResponse<LoginResponse>.SuccessResponse(loginResponse);

        _authServiceMock.Setup(x => x.LoginAsync(loginRequest))
            .ReturnsAsync(apiResponse);
        _environmentMock.Setup(x => x.EnvironmentName).Returns("Development");

        await _controller.Login(loginRequest);

        var cookies = _controller.Response.Headers["Set-Cookie"];
        cookies.Should().NotBeEmpty();
    }
}


using FluentAssertions;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HolidaysAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly AuthService _service;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public AuthServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("MySecretKeyForJWTTokenGeneration12345678901234567890");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("HolidaysAPI");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("HolidaysAPIUsers");

        _service = new AuthService(_mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "admin123"
        };

        var result = await _service.LoginAsync(request);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenCredentialsAreInvalid()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "wrongpassword"
        };

        var result = await _service.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenUsernameIsEmpty()
    {
        var request = new LoginRequest
        {
            Username = "",
            Password = "admin123"
        };

        var result = await _service.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Username and password are required");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenPasswordIsEmpty()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = ""
        };

        var result = await _service.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Username and password are required");
    }
}


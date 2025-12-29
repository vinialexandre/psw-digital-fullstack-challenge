using FluentAssertions;
using HolidaysAPI.Application.DTOs;
using HolidaysAPI.Application.Services;
using HolidaysAPI.Domain.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace HolidaysAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var jwtSettings = new JwtSettings
        {
            Key = "MySecretKeyForJWTTokenGeneration12345678901234567890",
            Issuer = "HolidaysAPI",
            Audience = "HolidaysAPIUsers",
            ExpirationHours = 24
        };

        var authSettings = new AuthSettings
        {
            AdminUsername = "admin",
            AdminPassword = "admin"
        };

        var mockJwtOptions = new Mock<IOptions<JwtSettings>>();
        mockJwtOptions.Setup(x => x.Value).Returns(jwtSettings);

        var mockAuthOptions = new Mock<IOptions<AuthSettings>>();
        mockAuthOptions.Setup(x => x.Value).Returns(authSettings);

        _service = new AuthService(mockJwtOptions.Object, mockAuthOptions.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "admin"
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
            Password = "admin"
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


using FluentAssertions;
using HolidaysAPI.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace HolidaysAPI.Tests.Middlewares;

public class ExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock;
    private readonly ExceptionHandlerMiddleware _middleware;

    public ExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextDelegate()
    {
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ReturnsInternalServerError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new Exception("Test exception");
        };

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception");
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw exception;
        };

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ReturnsJsonResponse()
    {
        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new Exception("Test exception");
        };

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(responseBody);
        var responseText = await reader.ReadToEndAsync();

        responseText.Should().Contain("Internal server error");
        responseText.Should().Contain("Test exception");
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ResponseHasCamelCaseProperties()
    {
        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new Exception("Test exception");
        };

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(responseBody);
        var responseText = await reader.ReadToEndAsync();

        var jsonDoc = JsonDocument.Parse(responseText);
        jsonDoc.RootElement.TryGetProperty("success", out _).Should().BeTrue();
        jsonDoc.RootElement.TryGetProperty("message", out _).Should().BeTrue();
    }
}


using System.Text.Json;
using FluentAssertions;
using HolidaysAPI.Infrastructure.Cache;
using HolidaysAPI.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;

namespace HolidaysAPI.Tests.Infrastructure;

public class RedisCacheServiceTests : IDisposable
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _databaseMock;
    private readonly Mock<ILogger<RedisCacheService>> _loggerMock;
    private readonly Mock<IOptions<RedisSettings>> _settingsMock;
    private readonly RedisCacheService _cacheService;

    public RedisCacheServiceTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _databaseMock = new Mock<IDatabase>();
        _loggerMock = new Mock<ILogger<RedisCacheService>>();
        _settingsMock = new Mock<IOptions<RedisSettings>>();

        var settings = new RedisSettings
        {
            ConnectionString = "localhost:6379",
            Enabled = true,
            DefaultExpirationMinutes = 1440
        };

        _settingsMock.Setup(x => x.Value).Returns(settings);
        _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_databaseMock.Object);

        _cacheService = new RedisCacheService(_redisMock.Object, _settingsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheHit_ReturnsCachedValue()
    {
        var key = "test-key";
        var cachedValue = "cached-value";
        var serializedValue = JsonSerializer.Serialize(cachedValue);
        var redisValue = RedisValue.Unbox(serializedValue);

        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisValue);

        var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult("new-value"));

        result.Should().Be(cachedValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheMiss_CallsFactory()
    {
        var key = "test-key";
        var newValue = "new-value";
        var factoryCalled = false;

        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        _databaseMock.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var result = await _cacheService.GetOrCreateAsync(key, () =>
        {
            factoryCalled = true;
            return Task.FromResult(newValue);
        });

        factoryCalled.Should().BeTrue();
        result.Should().Be(newValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenRedisDisabled_CallsFactoryDirectly()
    {
        var settings = new RedisSettings { Enabled = false };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var service = new RedisCacheService(_redisMock.Object, _settingsMock.Object, _loggerMock.Object);
        var factoryCalled = false;

        var result = await service.GetOrCreateAsync("key", () =>
        {
            factoryCalled = true;
            return Task.FromResult("value");
        });

        factoryCalled.Should().BeTrue();
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenRedisThrowsException_FallsBackToFactory()
    {
        var key = "test-key";
        var newValue = "new-value";

        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisException("Connection failed"));

        var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(newValue));

        result.Should().Be(newValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithNullKey_ThrowsArgumentException()
    {
        Func<Task> act = async () => await _cacheService.GetOrCreateAsync(null!, () => Task.FromResult("value"));

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetOrCreateAsync_WithEmptyKey_ThrowsArgumentException()
    {
        Func<Task> act = async () => await _cacheService.GetOrCreateAsync("", () => Task.FromResult("value"));

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetOrCreateAsync_WithNullFactory_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await _cacheService.GetOrCreateAsync<string>("key", null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void Remove_CallsRedisKeyDelete()
    {
        var key = "test-key";

        _cacheService.Remove(key);

        _databaseMock.Verify(x => x.KeyDelete(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public void Remove_WithNullKey_ThrowsArgumentException()
    {
        Action act = () => _cacheService.Remove(null!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Remove_WhenRedisDisabled_DoesNotThrow()
    {
        var settings = new RedisSettings { Enabled = false };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var service = new RedisCacheService(_redisMock.Object, _settingsMock.Object, _loggerMock.Object);

        Action act = () => service.Remove("key");

        act.Should().NotThrow();
    }

    [Fact]
    public void Remove_WhenRedisThrowsException_LogsError()
    {
        var key = "test-key";

        _databaseMock.Setup(x => x.KeyDelete(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .Throws(new RedisException("Connection failed"));

        _cacheService.Remove(key);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullRedis_ThrowsArgumentNullException()
    {
        Action act = () => new RedisCacheService(null!, _settingsMock.Object, _loggerMock.Object);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        Action act = () => new RedisCacheService(_redisMock.Object, null!, _loggerMock.Object);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Action act = () => new RedisCacheService(_redisMock.Object, _settingsMock.Object, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenValueIsNull_DoesNotSetCache()
    {
        var key = "test-key";

        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        var result = await _cacheService.GetOrCreateAsync<string?>(key, () => Task.FromResult<string?>(null));

        result.Should().BeNull();
        _databaseMock.Verify(x => x.StringSetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenUnexpectedExceptionThrown_RethrowsException()
    {
        var key = "test-key";

        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        Func<Task> act = async () => await _cacheService.GetOrCreateAsync(key, () => Task.FromResult("value"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    public void Dispose()
    {
        _cacheService?.Dispose();
    }
}


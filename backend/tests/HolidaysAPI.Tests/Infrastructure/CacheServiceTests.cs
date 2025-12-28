using FluentAssertions;
using HolidaysAPI.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace HolidaysAPI.Tests.Infrastructure;

public class CacheServiceTests
{
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _cacheService = new CacheService(_memoryCacheMock.Object);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheHit_ReturnsCachedValue()
    {
        var key = "test-key";
        var cachedValue = "cached-value";
        object? outValue = cachedValue;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out outValue))
            .Returns(true);

        var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult("new-value"));

        result.Should().Be(cachedValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheMiss_CallsFactory()
    {
        var key = "test-key";
        var newValue = "new-value";
        object? outValue = null;
        var factoryCalled = false;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out outValue))
            .Returns(false);

        _memoryCacheMock.Setup(x => x.CreateEntry(key))
            .Returns(Mock.Of<ICacheEntry>());

        var result = await _cacheService.GetOrCreateAsync(key, () =>
        {
            factoryCalled = true;
            return Task.FromResult(newValue);
        });

        factoryCalled.Should().BeTrue();
        result.Should().Be(newValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenCacheMiss_StoresValueInCache()
    {
        var key = "test-key";
        var newValue = "new-value";
        object? outValue = null;
        ICacheEntry? cacheEntry = null;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out outValue))
            .Returns(false);

        _memoryCacheMock.Setup(x => x.CreateEntry(key))
            .Returns((object k) =>
            {
                var entry = new Mock<ICacheEntry>();
                entry.SetupProperty(e => e.Value);
                entry.SetupProperty(e => e.AbsoluteExpirationRelativeToNow);
                cacheEntry = entry.Object;
                return entry.Object;
            });

        await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(newValue));

        cacheEntry.Should().NotBeNull();
        cacheEntry!.Value.Should().Be(newValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCustomExpiration_UsesProvidedExpiration()
    {
        var key = "test-key";
        var newValue = "new-value";
        var expiration = TimeSpan.FromMinutes(30);
        object? outValue = null;
        ICacheEntry? cacheEntry = null;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out outValue))
            .Returns(false);

        _memoryCacheMock.Setup(x => x.CreateEntry(key))
            .Returns((object k) =>
            {
                var entry = new Mock<ICacheEntry>();
                entry.SetupProperty(e => e.Value);
                entry.SetupProperty(e => e.AbsoluteExpirationRelativeToNow);
                cacheEntry = entry.Object;
                return entry.Object;
            });

        await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(newValue), expiration);

        cacheEntry.Should().NotBeNull();
        cacheEntry!.AbsoluteExpirationRelativeToNow.Should().Be(expiration);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithoutCustomExpiration_UsesDefaultExpiration()
    {
        var key = "test-key";
        var newValue = "new-value";
        object? outValue = null;
        ICacheEntry? cacheEntry = null;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out outValue))
            .Returns(false);

        _memoryCacheMock.Setup(x => x.CreateEntry(key))
            .Returns((object k) =>
            {
                var entry = new Mock<ICacheEntry>();
                entry.SetupProperty(e => e.Value);
                entry.SetupProperty(e => e.AbsoluteExpirationRelativeToNow);
                cacheEntry = entry.Object;
                return entry.Object;
            });

        await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(newValue));

        cacheEntry.Should().NotBeNull();
        cacheEntry!.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void Remove_CallsMemoryCacheRemove()
    {
        var key = "test-key";

        _cacheService.Remove(key);

        _memoryCacheMock.Verify(x => x.Remove(key), Times.Once);
    }
}


﻿using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EasyCaching.UnitTests.CachingTests;

public class FasterKvCachingProviderTest : BaseCachingProviderTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FasterKvCachingProviderTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _defaultTs = TimeSpan.FromSeconds(30);
    }

    protected override IEasyCachingProvider CreateCachingProvider(Action<BaseProviderOptions> additionalSetup)
    {
        var services = new ServiceCollection();
        services.AddEasyCaching(x =>
            x.UseFasterKv(options =>
                {
                    options.SerializerName = "msg";
                    additionalSetup(options);
                })
                .WithMessagePack("msg")
        );
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetService<IEasyCachingProvider>();
    }

    [Fact]
    protected void Set_And_Get_Big_DataSet_Should_Succeed()
    {
        // set 10w key
        for (int i = 0; i < 10_0000; i++)
        {
            _provider.Set($"Key_{i}", $"Cache_{i}", _defaultTs);
        }

        for (int i = 0; i < 10_0000; i++)
        {
            var value = _provider.Get<string>($"Key_{i}");
            Assert.True(value.HasValue);
            Assert.Equal(value.Value, $"Cache_{i}");
        }
    }

    [Fact]
    protected async Task SetAsync_And_GetAsync_Big_DataSet_Should_Succeed()
    {
        // set 10w key
        for (int i = 0; i < 10_0000; i++)
        {
            await _provider.SetAsync($"Key_{i}", $"Cache_{i}", _defaultTs);
        }

        for (int i = 0; i < 10_0000; i++)
        {
            var value = await _provider.GetAsync<string>($"Key_{i}");
            Assert.True(value.HasValue);
            Assert.Equal(value.Value, $"Cache_{i}");
        }
    }

    protected override Task GetAsync_Parallel_Should_Succeed()
    {
        return Task.FromResult(1);
    }

    protected override void Get_Parallel_Should_Succeed()
    {
    }

    public override Task GetAllByPrefix_Async_Should_Throw_ArgumentNullException_When_Prefix_IsNullOrWhiteSpace(
       string preifx)
    {
        return Task.CompletedTask;
    }

    public override void GetAllByPrefix_Should_Throw_ArgumentNullException_When_CacheKey_IsNullOrWhiteSpace(
        string prefix)
    {
    }

    protected override Task Get_Count_Async_With_Prefix_Should_Succeed()
    {
        return Task.CompletedTask;
    }

    public override void RemoveByPattern_Should_Succeed()
    {
    }

    public override Task RemoveByPatternAsync_Should_Succeed()
    {
        return Task.CompletedTask;
    }

    public override Task RemoveByPrefix_Async_Should_Throw_ArgumentNullException_When_Prefix_IsNullOrWhiteSpace(
        string preifx)

    {
        return Task.CompletedTask;
    }

    public override void RemoveByPrefix_Should_Throw_ArgumentNullException_When_CacheKey_IsNullOrWhiteSpace(
    string prefix)
    {
    }

    protected override Task Get_Count_Async_Without_Prefix_Should_Succeed()
    {
        return Task.CompletedTask;
    }


    protected override void Get_Count_With_Prefix_Should_Succeed()
    {
    }

    protected override void Get_Count_Without_Prefix_Should_Succeed()
    {
    }

    protected override void GetByPrefix_Should_Succeed()
    {
    }

    protected override void GetByPrefix_With_Not_Existed_Prefix_Should_Return_Empty_Dict()
    {
    }

    protected override Task GetByPrefixAsync_Should_Succeed()
    {
        return Task.CompletedTask;
    }



    protected override Task GetByPrefixAsync_With_Not_Existed_Prefix_Should_Return_Empty_Dict()
    {
        return Task.CompletedTask;
    }

    protected override Task GetExpiration_Async_Should_Succeed()
    {
        return Task.CompletedTask;
    }

    protected override void GetExpiration_Should_Succeed()
    {
    }

    protected override void RemoveByPrefix_Should_Succeed()
    {
    }

    protected override Task RemoveByPrefixAsync_Should_Succeed()
    {
        return Task.CompletedTask;
    }

    [Fact]
    protected async Task Cache_Expired_Should_Succeed()
    {
        var key = "key_test";
        var key_sync = "key_test_sync";
        var value = "cache_Key_test";

        await _provider.SetAsync(key, value, TimeSpan.FromSeconds(1));
        _provider.Set(key_sync, value, TimeSpan.FromSeconds(1));

        var val = await _provider.GetAsync<string>(key);
        var val_sync = _provider.Get<string>(key_sync);

        Assert.True(val.HasValue);
        Assert.True(val.Value == value);
        Assert.True(val_sync.HasValue);
        Assert.True(val_sync.Value == value);

        await Task.Delay(2 * 1000);

        val = await _provider.GetAsync<string>(key);
        val_sync = _provider.Get<string>(key_sync);

        Assert.True(val.IsNull);
        Assert.True(val_sync.IsNull);
    }

    [Fact]
    protected async Task Cache_Expired_After_Set_Should_Succeed()
    {
        var key = "key_test";
        var value = "cache_Key_test";

        await _provider.SetAsync(key, value, TimeSpan.FromSeconds(1));
        var val = await _provider.GetAsync<string>(key);
        Assert.True(val.HasValue);

        await Task.Delay(2000);

        val = await _provider.GetAsync<string>(key);
        Assert.True(val.IsNull);

        await _provider.SetAsync(key, value, TimeSpan.FromSeconds(1));
        val = await _provider.GetAsync<string>(key);
        Assert.True(val.HasValue);
    }
}
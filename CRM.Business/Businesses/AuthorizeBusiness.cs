using CRM.Business.Contracts;
using CRM.Common.DTOs.Redis;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace CRM.Business.Businesses;

public class AuthorizeBusiness : IAuthorizeBusiness
{
    private readonly IDistributedCache _cache;

    public AuthorizeBusiness(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task AddToRedis(string username, CancellationToken cancellationToken)
    {
        var rnd = new Random();

        var redisData = new RedisDto()
        {
            Code = rnd.Next(10000, 99999),
            CreatedAt = DateTime.Now,
        };

        var jsonData = JsonSerializer.Serialize(redisData);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(username, jsonData, cacheOptions, cancellationToken);
    }

    public async Task<RedisDto> GetFromRedis(string username, CancellationToken cancellationToken)
    {
        var redisData = await _cache.GetStringAsync(username, cancellationToken);

        if (redisData is null)
        {
            return new RedisDto();
        }

        var test = JsonSerializer.Deserialize<RedisDto>(redisData);

        if (test is null || test.CreatedAt.AddMinutes(2) <= DateTime.Now)
        {
            return new RedisDto();
        }

        return test;
    }
}

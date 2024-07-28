

using CRM.Common.DTOs.Redis;

namespace CRM.Business.Contracts;

public interface IAuthorizeBusiness
{
    Task AddToRedis(string username, CancellationToken cancellationToken);

    Task<RedisDto> GetFromRedis(string username, CancellationToken cancellationToken);
}

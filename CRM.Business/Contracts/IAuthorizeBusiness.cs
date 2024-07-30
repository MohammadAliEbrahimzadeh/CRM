

using CRM.Common.DTOs;
using CRM.Common.DTOs.Authentication;
using CRM.Common.DTOs.Redis;

namespace CRM.Business.Contracts;

public interface IAuthorizeBusiness
{
    Task AddToRedis(string username, CancellationToken cancellationToken);

    Task<RedisDto> GetFromRedis(string username, CancellationToken cancellationToken);

    Task<CustomResponse> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken);
}

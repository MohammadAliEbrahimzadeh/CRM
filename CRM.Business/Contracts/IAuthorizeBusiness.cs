using CRM.Common.DTOs;
using CRM.Common.DTOs.Authentication;
using CRM.Common.DTOs.Redis;

namespace CRM.Business.Contracts;

public interface IAuthorizeBusiness
{
    Task<CustomResponse> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken);
}

using CRM.Common.DTOs;
using CRM.Common.DTOs.Authentication;
using CRM.Common.DTOs.Redis;

namespace CRM.Business.Contracts;

public interface IAuthorizeBusiness
{
    Task<CustomResponse> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken);

    Task<CustomResponse> SignInAsync(SignInDto dto, CancellationToken cancellationToken);

    Task<CustomResponse> VerifyCredentialsAsync(CredentialsDto dto, CancellationToken cancellationToken);

    Task<CustomResponse> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken);

    Task<CustomResponse> SendForgotPasswordCodeAsync(SendForgotPasswordEmailDto dto, CancellationToken cancellationToken);
}

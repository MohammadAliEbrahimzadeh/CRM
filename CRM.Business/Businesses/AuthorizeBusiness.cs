using CRM.Business.Contracts;
using CRM.Common.DTOs;
using CRM.Common.DTOs.Authentication;
using CRM.Common.DTOs.Redis;
using CRM.DataAccess.UnitOfWork;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;
using System.Net;
using CRM.Common.Extentions;
using CRM.Models.Models;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using CRM.Common.DTOs.RabbitMessage;
using HotChocolate.Subscriptions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRM.Business.Businesses;

public class AuthorizeBusiness : IAuthorizeBusiness
{
    private readonly IDistributedCache _cache;
    private readonly UnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IConfiguration _configuration;
    private readonly ITopicEventSender _eventSender;

    public AuthorizeBusiness(IDistributedCache cache, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint, ITopicEventSender eventSender, IConfiguration configuration)
    {
        _cache = cache;
        _unitOfWork = (UnitOfWork)unitOfWork;
        _publishEndpoint = publishEndpoint;
        _eventSender = eventSender;
        _configuration = configuration;
    }

    public async Task<CustomResponse> VerifyCredentialsAsync(CredentialsDto dto, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username!, cancellationToken);

        if (user is null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No User Was Found"
            };

        var result = await _cache.GetStringAsync(user.Username!, cancellationToken);

        if (result is not null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Code Is Already Sent"
            };

        if (!user.EmailConfirmed)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "User Email Is Not Confirmed"
            };

        var hash = Generators.HashPassword(dto.Password!, user.PasswordSalt!);

        if (!user.PasswordHash!.Equals(hash))
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No User Was Found"
            };

        var rnd = new Random();

        var number = rnd.Next(100000, 999999);

        await AddToRedis(dto.Username!, new RedisDto()
        {
            Code = number,
            CreatedAt = DateTime.Now
        }, cancellationToken);

        await _publishEndpoint.Publish(new RabbitMessageDto()
        {
            Code = number,
            ChannelType = Models.Enums.ChannelType.Email,
            Email = user.Email,
            NotificationType = Models.Enums.NotificationType.LoginTwoFactor,
            Username = dto.Username

        }, cancellationToken);

        return new CustomResponse()
        {
            Code = HttpStatusCode.OK,
            Message = "Code Was Sent"
        };
    }

    public async Task<CustomResponse> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken)
    {
        var userExistsByUsername = await _unitOfWork.UserRepository.UserExistsByUsernameAsync(dto.Username!, cancellationToken);

        if (userExistsByUsername)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Username Is Taken"
            };

        var userExistsByEmail = await _unitOfWork.UserRepository.UserExistsByEmailAsync(dto.Email!, cancellationToken);

        if (userExistsByEmail)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Email Is Taken"
            };

        var salt = Generators.RandomString(5);

        var rnd = new Random();

        var number = rnd.Next(100000, 999999);

        await AddToRedis(dto.Username!, new RedisDto()
        {
            Code = number,
            CreatedAt = DateTime.Now
        }, cancellationToken);

        await _publishEndpoint.Publish(new RabbitMessageDto()
        {
            Code = number,
            ChannelType = Models.Enums.ChannelType.Email,
            Email = dto.Email,
            NotificationType = Models.Enums.NotificationType.EmailConfirmation,
            Username = dto.Username

        }, cancellationToken);

        var user = new User()
        {
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = Generators.HashPassword(dto.Password!, salt),
            PasswordSalt = salt,
        };

        await _unitOfWork.AddAsync(user, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            UserRoles = user.UserRoles!.Select(ur => new UserRoleDto
            {
                RoleId = ur.RoleId
            }).ToList()
        };

        await SubscribeToEvent("OnUserCreated", userDto, cancellationToken);

        return new CustomResponse()
        {
            Code = HttpStatusCode.OK,
            Message = "User is Created"
        };
    }

    public async Task<CustomResponse> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username!, cancellationToken);

        if (user is null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No User Was Found"
            };

        var result = await _cache.GetStringAsync(user.Username!, cancellationToken);

        if (result is null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No Data Was Found"
            };

        var redisData = JsonSerializer.Deserialize<RedisDto>(result);

        if (redisData!.Code != dto.Code)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Wrong Code"
            };

        user.EmailConfirmed = true;

        _unitOfWork.Update(user);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new CustomResponse()
        {
            Code = HttpStatusCode.OK,
            Message = "Email Confirmed"
        };
    }

    public async Task<CustomResponse> SignInAsync(SignInDto dto, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetRolesByUsernameAsync(dto.Username!, cancellationToken);

        if (user is null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No Data Was Found"
            };

        var result = await _cache.GetStringAsync(dto.Username!, cancellationToken);

        if (result is null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No Data Was Found"
            };

        var redisData = JsonSerializer.Deserialize<RedisDto>(result);

        if (redisData!.Code != dto.Code)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Wrong Code"
            };

        await _cache.RemoveAsync(dto.Username!, cancellationToken);

        var claims = new ClaimsIdentity();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        foreach (var item in user.UserRoles!)
        {
            claims.AddClaim(new(ClaimTypes.Role, item.Role!.Name!));
        }

        claims.AddClaim(new(ClaimTypes.NameIdentifier, user.Id.ToString()!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds,
            Audience = _configuration.GetSection("Jwt:Audience").Value!,
            Issuer = _configuration.GetSection("Jwt:Issuer").Value!,
            IssuedAt = DateTime.Now,
        };

        var jwtHandler = new JwtSecurityTokenHandler();

        var token = jwtHandler.CreateToken(tokenDescriptor);

        return new CustomResponse()
        {
            Data = new JwtSecurityTokenHandler().WriteToken(token),
            Code = HttpStatusCode.OK,
            Message = "SignIn Was Successful"
        };
    }

    public async Task<CustomResponse> SendForgotPasswordCodeAsync(SendForgotPasswordEmailDto dto, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email!, cancellationToken);

        if (user is null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "No Data Was Found"
            };

        var result = await _cache.GetStringAsync(user.Username!, cancellationToken);

        if (result is not null)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "Code Is Already Sent"
            };

        if (!user.EmailConfirmed)
            return new CustomResponse()
            {
                Code = HttpStatusCode.NoContent,
                Message = "User Email Is Not Confirmed"
            };

        var rnd = new Random();

        var number = rnd.Next(100000, 999999);

        await AddToRedis(user.Username!, new RedisDto()
        {
            Code = number,
            CreatedAt = DateTime.Now
        }, cancellationToken);

        await _publishEndpoint.Publish(new RabbitMessageDto()
        {
            Code = number,
            ChannelType = Models.Enums.ChannelType.Email,
            Email = user.Email,
            NotificationType = Models.Enums.NotificationType.ForgotPassword,
            Username = user.Username

        }, cancellationToken);

        return new CustomResponse()
        {
            Code = HttpStatusCode.OK,
            Message = "Code Was Sent"
        };

    }


    #region [Private Method[s]]

    private async Task AddToRedis<T>(string key, T t, CancellationToken cancellationToken)
    {
        var jsonData = JsonSerializer.Serialize(t);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(key, jsonData, cacheOptions, cancellationToken);
    }

    private async Task SubscribeToEvent<T>(string topic, T t, CancellationToken cancellationToken)
    {
        await _eventSender.SendAsync(topic, t, cancellationToken);
    }

    #endregion
}

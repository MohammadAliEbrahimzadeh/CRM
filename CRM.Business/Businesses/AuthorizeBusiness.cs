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

namespace CRM.Business.Businesses;

public class AuthorizeBusiness : IAuthorizeBusiness
{
    private readonly IDistributedCache _cache;
    private readonly UnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ITopicEventSender _eventSender;

    public AuthorizeBusiness(IDistributedCache cache, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint, ITopicEventSender eventSender)
    {
        _cache = cache;
        _unitOfWork = (UnitOfWork)unitOfWork;
        _publishEndpoint = publishEndpoint;
        _eventSender = eventSender;
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

    public async Task<CustomResponse> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken)
    {
        var userExistsByUsername = await _unitOfWork.UserRepository.UserExistsByUsernameAsync(dto.Username!, cancellationToken);

        if (userExistsByUsername)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Username Is Taken"
            };

        var userExistsByEmail = await _unitOfWork.UserRepository.UserExistsByEmailAsync(dto.Username!, cancellationToken);

        if (userExistsByEmail)
            return new CustomResponse()
            {
                Code = HttpStatusCode.Conflict,
                Message = "Email Is Taken"
            };

        var salt = Generators.RandomString(5);

        var rnd = new Random();

        var number = rnd.Next(10000, 99999);

        var redisData = new RedisDto()
        {
            Code = number,
            CreatedAt = DateTime.Now,
        };

        var jsonData = JsonSerializer.Serialize(redisData);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(dto.Username!, jsonData, cacheOptions, cancellationToken);

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

        await _eventSender.SendAsync("OnUserCreated", user, cancellationToken);

        return new CustomResponse()
        {
            Code = HttpStatusCode.OK,
            Message = "User is Created"
        };
    }
}

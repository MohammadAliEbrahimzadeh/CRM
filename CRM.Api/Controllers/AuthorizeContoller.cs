﻿using CRM.Business.Contracts;
using CRM.Common.DTOs.Authentication;
using CRM.Common.Extentions;
using CRM.DataAccess.Context;
using CRM.Models.Models;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CRM.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizeContoller : ControllerBase
{
    private readonly CRMContext _context;
    private readonly ITopicEventSender _eventSender;
    private readonly IAuthorizeBusiness _authorizeBusiness;

    public AuthorizeContoller(CRMContext context, ITopicEventSender eventSender, IAuthorizeBusiness authorizeBusiness)
    {
        _context = context;
        _eventSender = eventSender;
        _authorizeBusiness = authorizeBusiness;
    }

    [HttpPost]
    [Route("SignUp")]
    public async Task<IActionResult> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var salt = Generators.RandomString(5);

            var user = new User()
            {
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = Generators.HashPassword(dto.Password!, salt),
                PasswordSalt = salt,
            };

            await _context.Users!.AddAsync(user, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

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

            await _context.SaveChangesAsync(cancellationToken);

            await _eventSender.SendAsync("OnUserCreated", user, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    [HttpPost]
    [Route("AddToRedis")]
    public async Task<IActionResult> AddToRedis(string username, CancellationToken cancellation)
    {
        await _authorizeBusiness.AddToRedis(username, cancellation);

        return Ok();
    }


    [HttpGet]
    [Route("GetFromRedis")]
    public async Task<IActionResult> GetFromRedis(string username, CancellationToken cancellation)
    {
        var result = await _authorizeBusiness.GetFromRedis(username, cancellation);

        return Ok(result);
    }
}
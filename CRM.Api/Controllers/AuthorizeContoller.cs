using CRM.Business.Contracts;
using CRM.Common.DTOs;
using CRM.Common.DTOs.Authentication;
using CRM.Common.DTOs.RabbitMessage;
using CRM.Common.Extentions;
using CRM.DataAccess.Context;
using CRM.Models.Models;
using HotChocolate.Subscriptions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CRM.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizeController : ControllerBase
{
    private readonly CRMContext _context;
    private readonly ITopicEventSender _eventSender;
    private readonly IAuthorizeBusiness _authorizeBusiness;
    private readonly IConfiguration _configuration;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthorizeController(CRMContext context, ITopicEventSender eventSender, IAuthorizeBusiness authorizeBusiness, IConfiguration configuration, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _eventSender = eventSender;
        _authorizeBusiness = authorizeBusiness;
        _configuration = configuration;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    [Route("SignUp")]
    public async Task<CustomResponse> SignUpAsync(AddUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return await _authorizeBusiness.SignUpAsync(dto, cancellationToken);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    [HttpPost]
    [Route("VerifyCredentials")]
    public async Task<CustomResponse> VerifyCredentialsAsync(CredentialsDto dto, CancellationToken cancellationToken)
    {
        return await _authorizeBusiness.VerifyCredentialsAsync(dto, cancellationToken);
    }

    [HttpPut]
    [Route("VerifyEmail")]
    public async Task<CustomResponse> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return await _authorizeBusiness.VerifyEmailAsync(dto, cancellationToken);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    [HttpPost]
    [Route("ExceptionTest")]
    public async Task<IActionResult> ExceptionTest(CancellationToken cancellationToken)
    {
        throw new Exception("this is test");
    }
}

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
public class AuthorizeContoller : ControllerBase
{
    private readonly CRMContext _context;
    private readonly ITopicEventSender _eventSender;
    private readonly IAuthorizeBusiness _authorizeBusiness;
    private readonly IConfiguration _configuration;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthorizeContoller(CRMContext context, ITopicEventSender eventSender, IAuthorizeBusiness authorizeBusiness, IConfiguration configuration, IPublishEndpoint publishEndpoint)
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
    [Route("SendEmail")]
    public async Task<IActionResult> SendEmail(CancellationToken cancellation)
    {
        var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "Email", "Confirmation.html");

        if (!Path.Exists(emailTemplatePath))
        {
            return BadRequest();
        }

        var rnd = new Random();

        var num = rnd.Next(10000, 99999);

        //var email = new Sender(_configuration);

        //await email.SendEmailAsync("mohammad77.me@gmail.com", "Confirmation", emailTemplatePath, "Asghar", num.ToString(), cancellation);

        return Ok();
    }

    [HttpPost]
    [Route("RabbitMqTest")]
    public async Task<IActionResult> RabbitMqTest(CancellationToken cancellationToken)
    {
        var rnd = new Random();
        var num = rnd.Next(10000, 99999);

        await _publishEndpoint.Publish(new RabbitMessageDto()
        {
            ChannelType = Models.Enums.ChannelType.Email,
            Code = num,
            Email = "mohammad77.me@gmail.com",
            NotificationType = Models.Enums.NotificationType.EmailConfirmation,
            Username = "Mohammad"
        }, cancellationToken);

        return Ok();
    }
}

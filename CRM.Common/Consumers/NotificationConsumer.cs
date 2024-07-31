using CRM.Common.DTOs.RabbitMessage;
using CRM.Common.Extentions;
using CRM.Models.Enums;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace CRM.Common.Consumers;

public class NotificationConsumer : IConsumer<RabbitMessageDto>
{
    private readonly IConfiguration _configuration;

    public NotificationConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<RabbitMessageDto> context)
    {
        var message = context.Message;

        switch (message.NotificationType)
        {
            case NotificationType.ForgotPassword:
                break;
            case NotificationType.EmailConfirmation:
                await SendConfirmationEmail(message.Email!, "Confirmation", message.Username!, message.Code.ToString()!);
                break;
            case NotificationType.SalesUpdate:
                break;
            case NotificationType.AccountActivation:
                break;
            case NotificationType.PasswordChange:
                break;
            case NotificationType.LoginTwoFactor:
                await SendLoginTwoFactorEmail(message.Email!, "Login Two Factor Code", message.Username!, message.Code.ToString()!);
                break;
            default:
                break;
        }
    }

    private async Task SendConfirmationEmail(string toAddress, string subject, string name, string code)
    {
        var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "Email", "Confirmation.html");

        if (File.Exists(emailTemplatePath))
        {
            var emailSender = new Sender(_configuration);

            await emailSender.SendEmailAsync(toAddress, subject, emailTemplatePath, name, code);
        }

        else
        {
            throw new Exception("Template Not Found");
        }

    }

    private async Task SendLoginTwoFactorEmail(string toAddress, string subject, string name, string code)
    {
        var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "Email", "TwoFactorLogin.html");

        if (File.Exists(emailTemplatePath))
        {
            var emailSender = new Sender(_configuration);

            await emailSender.SendEmailAsync(toAddress, subject, emailTemplatePath, name, code);
        }

        else
        {
            throw new Exception("Template Not Found");
        }

    }
}

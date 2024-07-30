using CRM.Models.Enums;
using MailKit.Net.Smtp;
using MassTransit;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Xml.Linq;

namespace CRM.Common.Extentions;

public class Sender
{
    private readonly IConfiguration _configuration;

    public Sender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync

        (string toAddress, string subject, string path, string name, string? code)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("CRM", _configuration.GetSection("EmailConfiguration:Address").Value));

        message.To.Add(new MailboxAddress(string.Empty, toAddress));

        message.Subject = subject;

        var template = await File.ReadAllTextAsync(path);

        var messageBody = !string.IsNullOrEmpty(code)
              ? template.Replace("{{Name}}", name).Replace("{{Code}}", code)
              : template.Replace("{{Name}}", name);

        // Connect and send the email using Gmail's SMTP server
        var client = new SmtpClient();

        // Connect to Gmail SMTP server
        await client.ConnectAsync("smtp.gmail.com", 465, true);

        // Authenticate using your email and app password
        await client.AuthenticateAsync(_configuration.GetSection("EmailConfiguration:Address").Value,
            _configuration.GetSection("EmailConfiguration:PassKey").Value);

        // Send the email
        await client.SendAsync(message);
        // Disconnect and quit
        await client.DisconnectAsync(true);
    }
}

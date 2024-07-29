using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common.Extentions;

public class Sender
{
    private readonly string _templatePath;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public Sender(string templatePath, string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        _templatePath = templatePath;
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPass = smtpPass;
    }

    public void SendEmailAsync(string to, string subject, string body)
    {
        var fromAddress = new MailAddress(_smtpUser, "Your Company");
        var toAddress = new MailAddress(to);
        var smtp = new SmtpClient
        {
            Host = _smtpServer,
            Port = _smtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_smtpUser, _smtpPass)
        };

        var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        smtp.Send(message);
    }


    public async Task<string> GenerateEmailBody(string name, string code)
    {
        var template = await File.ReadAllTextAsync(_templatePath);
        var body = template.Replace("{{Name}}", name)
                           .Replace("{{Code}}", code);
        return body;
    }
}

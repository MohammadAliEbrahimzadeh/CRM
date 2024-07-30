using CRM.Models.Enums;

namespace CRM.Common.DTOs.RabbitMessage;

public class RabbitMessageDto
{
    public ChannelType ChannelType { get; set; }

    public NotificationType NotificationType { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public int? Code { get; set; }
}

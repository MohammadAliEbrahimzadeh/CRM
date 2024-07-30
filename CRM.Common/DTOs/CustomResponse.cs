using System.Net;

namespace CRM.Common.DTOs;

public class CustomResponse
{
    public string? Message { get; set; }

    public HttpStatusCode Code { get; set; }

    public dynamic? Data { get; set; }
}

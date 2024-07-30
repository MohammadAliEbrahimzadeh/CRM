using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net;
using Serilog;

namespace CRM.Web;

public class ExceptionHandler
{
    private readonly RequestDelegate _delegate;

    public ExceptionHandler(RequestDelegate @delegate)
    {
        _delegate = @delegate;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _delegate(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Error = "An unexpected error occurred."
            }));
        }
    }
}

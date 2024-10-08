using CRM.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .InjectAddEndpointsApiExplorer()
    .InjectControllers()
    .InjectAddSwaggerGen()
    .InjectContext(builder.Configuration)
    .InjectRedisCache(builder.Configuration)
    .InjectHotChocolate()
    .InjectHealthCheck(builder.Configuration)
    .InjectFluentValidation()
    .InjectBusinesses()
    .InjectUnitOfWork()
    .InjectMassTransit(builder.Configuration)
    .InjectLogger(builder.Configuration)
    .InjectAuthentication(builder.Configuration)
    .InjectHttpContextAccessor()
    .InjectGraphQlQueries();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM V1");
});

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandler>();

app.UseWebSockets();

app.MapControllers();

app.MapGraphQL("/graphql");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthCheckResponseWriter.WriteHealthCheckResponse
});

await app.RunAsync();


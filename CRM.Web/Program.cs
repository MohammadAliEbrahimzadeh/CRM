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
    .InjectBusinesses();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.MapControllers();

app.MapGraphQL("/graphql");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthCheckResponseWriter.WriteHealthCheckResponse
});

await app.RunAsync();


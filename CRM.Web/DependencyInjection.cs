using CRM.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using CRM.GraphQL.Queries;
using CRM.GraphQL.Types;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using CRM.GraphQL.Subscriptions;
using Newtonsoft.Json;
using FluentValidation;
using CRM.Common.DTOs.Authentication;
using FluentValidation.AspNetCore;
using CRM.Business.Contracts;
using CRM.Business.Businesses;

namespace CRM.Web;

internal static class DependencyInjection
{
    internal static IServiceCollection InjectContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContextFactory<CRMContext>(optionsAction =>
        {
            optionsAction.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

    internal static IServiceCollection InjectAddEndpointsApiExplorer(this IServiceCollection services) =>
       services.AddEndpointsApiExplorer();

    internal static IServiceCollection InjectControllers(this IServiceCollection services) =>
        services.AddControllers()
                .Services;

    internal static IServiceCollection InjectAddSwaggerGen(this IServiceCollection services) =>
       services.AddSwaggerGen();

    internal static IServiceCollection InjectHotChocolate(this IServiceCollection services) =>
      services
        .AddGraphQLServer()
        .AddQueryType<UserQueries>()
        .AddType<UserType>()
        .AddType<UserRoleType>()
        .AddProjections()
        .AddFiltering()
        .AddSubscriptionType<UserSubscription>()
        .AddRedisSubscriptions((sp) => ConnectionMultiplexer.Connect("localhost:6379"))
        .Services;

    internal static IServiceCollection InjectRedisCache(this IServiceCollection services, IConfiguration configuration) =>
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("RedisConfiguration:Connection").Value;
            options.InstanceName = "OtpCodes-";
        });

    internal static IServiceCollection InjectHealthCheck(this IServiceCollection services, IConfiguration configuration) =>
        services
        .AddHealthChecks()
        .AddDbContextCheck<CRMContext>("CRMContext", HealthStatus.Unhealthy)
        .AddRedis(configuration.GetSection("RedisConfiguration:Connection").Value!, "Redis", HealthStatus.Unhealthy).Services;

    internal static IServiceCollection InjectFluentValidation(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<AddUserDto>().AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

    internal static IServiceCollection InjectBusinesses(this IServiceCollection services) =>
        services.AddScoped<IAuthorizeBusiness, AuthorizeBusiness>();
}

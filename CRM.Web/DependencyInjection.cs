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
using CRM.DataAccess.UnitOfWork;
using MassTransit;
using CRM.Common.Consumers;
using Serilog.Sinks.MongoDB;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace CRM.Web;

internal static class DependencyInjection
{
    internal static IServiceCollection InjectContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContextFactory<CRMContext>(optionsAction =>
        {
            optionsAction.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

    internal static IServiceCollection InjectHttpContextAccessor(this IServiceCollection services) =>
       services.AddHttpContextAccessor();

    internal static IServiceCollection InjectAddEndpointsApiExplorer(this IServiceCollection services) =>
       services.AddEndpointsApiExplorer();

    internal static IServiceCollection InjectControllers(this IServiceCollection services) =>
        services.AddControllers()
                .Services;

    internal static IServiceCollection InjectAddSwaggerGen(this IServiceCollection services) =>
       services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM", Version = "v1" });

           c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
           {
               Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
               Name = "Authorization",
               In = ParameterLocation.Header,
               Type = SecuritySchemeType.ApiKey,
               Scheme = "Bearer"
           });

           c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
       });

    internal static IServiceCollection InjectHotChocolate(this IServiceCollection services) =>
      services
        .AddGraphQLServer()
        .AddQueryType<UserQueries>()
        .AddType<UserType>()
        .AddType<UserRoleType>()
        .AddProjections()
        .AddFiltering()
        .AddAuthorization()
        .AddSorting()
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
        .AddRedis(configuration.GetSection("RedisConfiguration:Connection").Value!, "Redis", HealthStatus.Unhealthy)
        .AddMongoDb(configuration.GetSection("Mongo:Connection").Value!).Services;

    internal static IServiceCollection InjectFluentValidation(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<AddUserDto>().AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

    internal static IServiceCollection InjectBusinesses(this IServiceCollection services) =>
        services.AddScoped<IAuthorizeBusiness, AuthorizeBusiness>();

    internal static IServiceCollection InjectUnitOfWork(this IServiceCollection services) =>
        services.AddScoped<IUnitOfWork, UnitOfWork>();

    internal static IServiceCollection InjectMassTransit(this IServiceCollection services, IConfiguration configuration) =>
        services.AddMassTransit(x =>
        {
            x.AddConsumer<NotificationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetSection("RabbitMQ:Host").Value, "/", h =>
                {
                    h.Username(configuration.GetSection("RabbitMQ:Username").Value!);
                    h.Password(configuration.GetSection("RabbitMQ:Password").Value!);
                });

                cfg.ReceiveEndpoint("Notifications", e =>
                {
                    e.ConfigureConsumer<NotificationConsumer>(context);
                });

                cfg.UseMessageRetry(configuration =>
                {
                    configuration.Interval(5, TimeSpan.FromSeconds(10));
                });
            });
        });

    internal static IServiceCollection InjectLogger(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.MongoDB(configuration.GetSection("Mongo:Connection").Value!, configuration.GetSection("Mongo:Collection").Value!)
            .WriteTo.File(
                    path: "logs/log.txt",
                    rollingInterval: Serilog.RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
            .MinimumLevel.Error()
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        return services;

    }

    internal static IServiceCollection InjectAuthentication(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddAuthentication(options =>
             {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
            {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration.GetSection("Jwt:Issuer").Value!,
            ValidAudience = configuration.GetSection("Jwt:Audience").Value!,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value!))
            };
            }).Services;
}

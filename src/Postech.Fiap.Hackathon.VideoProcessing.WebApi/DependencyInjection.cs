using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Behavior;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Authentication.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queue;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Persistence;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    private static readonly Assembly Assembly = typeof(Program).Assembly;

    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlServerConnectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(sqlServerConnectionString,
                options => { options.EnableRetryOnFailure(2, TimeSpan.FromSeconds(3), new List<int>()); });
        });

        services.AddSwaggerConfiguration();
        services.AddIdentityConfiguration();
        services.AddMediatRConfiguration();
        services.AddOpenTelemetryConfiguration();
        services.AddJsonOptionsConfiguration();

        services.AddProblemDetails();
        services.AddCarter();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddUseHealthChecksConfiguration(configuration);
        services.AddValidatorsFromAssembly(Assembly);

        services.AddSingleton<CloudBlobContainer>(sp =>
        {
            var connectionString = configuration["Azure:ConnectionString"];
            var containerName = configuration["Azure:Blob:Container"];

            var account = CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();

            return container;
        });

        services.AddScoped<IStorageService, StorageService>();

        services.AddSingleton<CloudQueue>(_ =>
        {
            var connectionString = configuration["Azure:ConnectionString"];
            var queueName = configuration["Azure:Queue:Name"];

            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudQueueClient();

            var queue = client.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return queue;
        });

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 500 * 1024 * 1024; // 500 MB
        });


        services.AddScoped<IVideoQueueMessenger, VideoQueueMessenger>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<IVideoRepository, VideoRepository>();

        return services;
    }

    private static void AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    private static void AddOpenTelemetryConfiguration(this IServiceCollection services)
    {
        var serviceName = $"{Assembly.GetName().Name}-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
        services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService(serviceName!))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();

                metrics.AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddEntityFrameworkCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();

                tracing.AddOtlpExporter();
            });
    }

    private static void AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        services.AddAuthorization();

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Fiap Hackathon Video Processing WebApi",
                Description = "API para processamento de vídeos",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT no formato: Bearer {seu token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    }

    public static void AddSerilogConfiguration(this IServiceCollection services,
        WebApplicationBuilder builder, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var applicationName =
            $"{Assembly.GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}";

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", applicationName)
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);
    }

    public static IServiceCollection AddJsonOptionsConfiguration(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
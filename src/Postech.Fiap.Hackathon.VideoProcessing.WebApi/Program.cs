using Postech.Fiap.Hackathon.VideoProcessing.WebApi;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Middleware;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Authentication.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = AppSettings.Configuration();

builder.Services
    .AddWebApi(configuration)
    .AddSerilogConfiguration(builder, configuration);

var app = builder.Build();

app.ApplyMigrations();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseHealthChecksConfiguration();

app.MapGroup("/api/auth")
    .WithTags("Authentication")
    .MapIdentityApi<User>()
    .WithDescription("Authentication API");

app.MapCarter();

app.Run();
using Postech.Fiap.Hackathon.VideoProcessing.WebApi;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Authentication.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Settings;

var builder = WebApplication.CreateBuilder(args);
var configuration = AppSettings.Configuration();

builder.Services
    .AddWebApi(configuration);

var app = builder.Build();

app.ApplyMigrations();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGroup("/api/auth")
    .WithTags("Authentication")
    .MapIdentityApi<User>()
    .WithDescription("Authentication API");

app.Run();
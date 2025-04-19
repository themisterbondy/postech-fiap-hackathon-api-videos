using Microsoft.AspNetCore.Mvc;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queries;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Endpoints;

[ExcludeFromCodeCoverage]
public class VideoEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/videos");

        group.MapGet("/{id:Guid}", async (Guid id, [FromServices] IMediator mediator) =>
            {
                var query = new GetStatusVideoById.Query
                {
                    Id = id
                };

                var result = await mediator.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : result.ToProblemDetails();
            })
            .WithName("GetStatusVideoById")
            .Produces<GetStatusVideoResponse>()
            .WithTags("Video")
            .RequireAuthorization()
            .WithOpenApi();


        group.MapPost("/upload", async ([FromForm] UploadVideoRequest request, [FromServices] IMediator mediator) =>
            {
                var command = new UploadVideoCommand.Command
                {
                    File = request.File,
                    ThumbnailsInterval = request.ThumbnailsInterval
                };

                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? Results.Created($"/Video/{result.Value.Id}", result.Value)
                    : result.ToProblemDetails();
            })
            .WithName("CreateVideo")
            .DisableAntiforgery()
            .Produces<UploadVideoResponse>(204)
            .WithTags("Video")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/{id:Guid}/download",
                async (Guid id, [FromServices] IMediator mediator) =>
                {
                    var command = new DownloadVideoCommand.Command
                    {
                        Id = id
                    };

                    var result = await mediator.Send(command);

                    return result.IsSuccess
                        ? Results.File(
                            result.Value.File,
                            result.Value.ContentType,
                            result.Value.FileName)
                        : result.ToProblemDetails();
                })
            .WithName("UploadVideo")
            .WithTags("Video")
            .RequireAuthorization()
            .WithOpenApi();
    }
}
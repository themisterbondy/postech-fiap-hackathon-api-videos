using Microsoft.AspNetCore.Mvc;
using Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
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
          .Produces<GetStatusVideoResponse>(200)
          .WithTags("Video")
          .RequireAuthorization()
          .WithOpenApi();


        group.MapPost("/upload", async ([FromBody] UploadVideoRequest request, [FromServices] IMediator mediator) =>
            {
                var command = new UploadVideoCreate.Command
                {
                    File = request.File
                };

                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? Results.Created($"/api/videos/{result.Value.Id}", result.Value)
                    : result.ToProblemDetails();
            })
            .WithName("UploadVideo")
            .Accepts<UploadVideoRequest>("application/json")
            .Produces<UploadVideoResponse>(201)
            .WithTags("Video")
            .RequireAuthorization()
            .WithOpenApi();

    }
}

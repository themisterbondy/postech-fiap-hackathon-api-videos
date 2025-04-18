using Microsoft.AspNetCore.Mvc;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;
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
          .WithTags("Products")
          .WithOpenApi();
    }



}

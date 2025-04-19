using Microsoft.AspNetCore.Mvc;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Extensions;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queries;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Endpoints;

/// <summary>
/// Endpoints para gerenciamento e processamento de vídeos na aplicação.
/// Fornece funcionalidades para verificar status, fazer upload e download de vídeos.
/// </summary>
[ExcludeFromCodeCoverage]
public class VideoEndpoints : ICarterModule
{
    /// <summary>
    /// Configura as rotas para os endpoints de vídeo.
    /// </summary>
    /// <param name="app">O construtor de rotas de endpoint.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/videos");

        /// <summary>
        /// Endpoint para obter o status de processamento de um vídeo específico.
        /// </summary>
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
            .WithOpenApi(operation =>
            {
                operation.Summary = "Obtém o status de processamento de um vídeo";
                operation.Description =
                    "Retorna informações sobre o status atual de processamento de um vídeo específico pelo seu ID";
                return operation;
            });

        /// <summary>
        /// Endpoint para fazer upload de um novo vídeo para processamento.
        /// </summary>
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
            .Produces<UploadVideoResponse>(201)
            .WithTags("Video")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Faz upload de um novo vídeo";
                operation.Description =
                    "Permite enviar um arquivo de vídeo para processamento, definindo o intervalo para geração de miniaturas";
                return operation;
            });

        /// <summary>
        /// Endpoint para fazer download de um vídeo processado.
        /// </summary>
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
            .WithName("DownloadVideo")
            .WithTags("Video")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Faz download de um vídeo";
                operation.Description =
                    "Permite baixar um vídeo processado pelo seu ID, retornando o arquivo para o cliente";
                return operation;
            });
    }
}
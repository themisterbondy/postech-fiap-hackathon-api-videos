using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;

public abstract class DownloadVideoCommand
{
    public class Command : IRequest<Result<DownloadVideoZipResponse>>
    {
        public Guid Id { get; set; }
    }

    public class DownloadVideoCommandHandler(IVideoService videoService)
        : IRequestHandler<Command, Result<DownloadVideoZipResponse>>
    {
        public async Task<Result<DownloadVideoZipResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await videoService.Download(request.Id, cancellationToken);
            return result.IsFailure
                ? Result.Failure<DownloadVideoZipResponse>(result.Error)
                : Result.Success(new DownloadVideoZipResponse(result.Value.File, result.Value.ContentType,
                    result.Value.FileName));
        }
    }
}
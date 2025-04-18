using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

namespace Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;

public abstract class DownloadVideo
{
    public class Command : IRequest<Result<DownloadVideoZipResponse>>
    {
        public Guid Id { get; set; }
    }

    public class DownloadVideoHandler(IVideoService videoService)
        : IRequestHandler<Command, Result<DownloadVideoZipResponse>>
    {
        public async Task<Result<DownloadVideoZipResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await videoService.download(request.Id, cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure<DownloadVideoZipResponse>(Error.Failure("DownloadVideoHandler", "File Error"));
                ;
            }

            return Result.Success(new DownloadVideoZipResponse(result.Value.File, result.Value.ContentType,
                result.Value.FileName));
        }
    }
}
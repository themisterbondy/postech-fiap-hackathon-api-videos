using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;

public abstract class UploadVideoCommand
{
    public class Command : IRequest<Result<UploadVideoResponse>>
    {
        public int ThumbnailsInterval { get; set; }
        public IFormFile? File { get; set; }
    }

    public class UploadVideoCommandHandler(IVideoService videoService)
        : IRequestHandler<Command, Result<UploadVideoResponse>>
    {
        public async Task<Result<UploadVideoResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var response = await videoService.Upload(request, cancellationToken);
            return response.IsFailure
                ? Result.Failure<UploadVideoResponse>(response.Error)
                : Result.Success(response.Value);
        }
    }
}
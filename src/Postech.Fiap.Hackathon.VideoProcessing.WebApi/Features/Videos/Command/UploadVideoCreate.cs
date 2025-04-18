using FluentValidation;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Validation;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

namespace Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;


public abstract class UploadVideoCreate
{

    public class Command : IRequest<Result<UploadVideoResponse>>
    {
        public IFormFile File { get; set; }
    }

    public class CreateVideotHandler(IVideoService videoService)
           : IRequestHandler<Command, Result<UploadVideoResponse>>
    {
        public async Task<Result<UploadVideoResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var uploadRequest = new UploadVideoRequest
            {
                File = request.File
            };

            var response = await videoService.upload(uploadRequest, cancellationToken);

            return Result.Success(new UploadVideoResponse(response.Value.Id,response.Value.Status));
        }
    }

}
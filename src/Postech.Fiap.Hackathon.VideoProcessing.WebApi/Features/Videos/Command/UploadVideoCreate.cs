using FluentValidation;
using OpenTelemetry.Trace;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.Validation;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

namespace Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;


public abstract class UploadVideoCreate
{

    public class Command : IRequest<Result<UploadVideoResponse>>
    {
        public Guid Id { get; set; }
        public VideoStatus Status { get; set; }

        public IFormFile? File { get; set; }
    }

    public class CreateVideotHandler(IVideoRepository videoRepository)
           : IRequestHandler<Command, Result<UploadVideoResponse>>
    {
        public async Task<Result<UploadVideoResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            await videoRepository.AddAsync(
                  Video.Create(
                    Guid.NewGuid(),
                    0 // Replace with appropriate value for ThumbnailsInterval
                ));
            return Result.Success(new UploadVideoResponse());
        }
    }

    public class UpdateVideoValidator : AbstractValidator<Command>
    {
        public UpdateVideoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithError(Error.Validation("Id", "ID is required."));
            RuleFor(x => x.Status)
                .IsInEnum().WithError(Error.Validation("Category", "Category is invalid."));
        }
    }
}
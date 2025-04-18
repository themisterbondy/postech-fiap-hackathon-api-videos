using Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IVideoService
{
    Task<Result<GetStatusVideoResponse>> getVideoById(Guid id, CancellationToken cancellationToken);

    Task<Result<UploadVideoResponse>> upload(UploadVideoCreate.Command request, CancellationToken cancellationToken);

    Task<Result<DownloadVideoZipResponse>> download(Guid id, CancellationToken cancellationToken);
}
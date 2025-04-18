using Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IVideoService
{
    Task<Result<GetStatusVideoResponse>> getVideoById(Guid videoId, CancellationToken cancellationToken);

    Task<Result<UploadVideoResponse>> upload(UploadVideoCreate.Command request, CancellationToken cancellationToken);

    Task<Result<DownloadVideoZipResponse>> download(Guid videoId, CancellationToken cancellationToken);
}
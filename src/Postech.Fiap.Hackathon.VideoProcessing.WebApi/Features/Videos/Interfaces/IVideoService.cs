using Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IVideoService
{
    Task<Result<GetStatusVideoResponse>> GetVideoById(Guid videoId, CancellationToken cancellationToken);

    Task<Result<UploadVideoResponse>> Upload(UploadVideoCommand.Command request, CancellationToken cancellationToken);

    Task<Result<DownloadVideoZipResponse>> Download(Guid videoId, CancellationToken cancellationToken);
}
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;


public interface IVideoService
{
    Task<Result<GetStatusVideoResponse>> getVideoById(Guid id, CancellationToken cancellationToken);

    Task<Result<UploadVideoResponse>> upload(UploadVideoRequest request, CancellationToken cancellationToken);

    Task<Result<DownloadVideoZipResponse>> download( DownloadVideoZipRequest request, CancellationToken cancellationToken);
}
    
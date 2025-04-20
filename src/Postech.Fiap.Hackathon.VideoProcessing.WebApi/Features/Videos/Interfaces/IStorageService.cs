using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IStorageService
{
    Task<Result<string>> UploadVideoAsync(Guid videoId, Stream videoStream, string contentType,
        CancellationToken cancellationToken);

    Task<Result<Stream>> DownloadVideoAsync(string filePath, CancellationToken cancellationToken);
}
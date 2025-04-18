using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IStorageService
{
    Task<Result<string>> UploadAsync(Guid videoId, Stream videoStream, string contentType);
    Task<Result<Stream>> DowloadAsync(string FilePath);
}
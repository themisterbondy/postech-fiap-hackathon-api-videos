namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IVideoUploader
{
    Task UploadAsync(Guid videoId, Stream videoStream, string contentType);
}
using Microsoft.Azure.Storage.Blob;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queue;

public class VideoUploader(CloudBlobContainer container) : IVideoUploader
{
    public async Task UploadAsync(Guid videoId, Stream videoStream, string contentType)
    {
        var blobName = $"{videoId}.mp4";
        var blob = container.GetBlockBlobReference(blobName);
        blob.Properties.ContentType = contentType;

        await blob.UploadFromStreamAsync(videoStream);
    }
}
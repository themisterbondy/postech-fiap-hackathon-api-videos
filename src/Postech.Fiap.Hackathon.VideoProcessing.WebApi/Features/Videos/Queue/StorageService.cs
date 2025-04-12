using Azure;
using Microsoft.Azure.Storage.Blob;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queue;

public class StorageService(CloudBlobContainer container) : IStorageService
{
    public async Task<Result<string>> UploadAsync(Guid videoId, Stream videoStream, string contentType)
    {
        const string errorCode = "StorageService.UploadAsync";
        try
        {
            var blobName = $"{videoId}/{videoId}.mp4";
            var blob = container.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = contentType;

            await blob.UploadFromStreamAsync(videoStream);

            return blob.Uri.ToString();
        }
        catch (RequestFailedException ex)
        {
            return Result.Failure<string>(Error.Failure(errorCode, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(Error.Failure(errorCode, ex.Message));
        }
    }
}
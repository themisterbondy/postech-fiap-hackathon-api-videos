using Azure;
using Microsoft.Azure.Storage.Blob;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

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

            return blobName;
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

    public async Task<Result<Stream>> DowloadAsync(string FilePath)
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            return Result.Failure<Stream>(Error.Failure("StorageService.DowloadAsync",
                "File path cannot be null or empty"));
        }

        try
        {
            var blob = container.GetBlockBlobReference(FilePath);
            if (await blob.ExistsAsync())
            {
                var stream = new MemoryStream();
                await blob.DownloadToStreamAsync(stream);
                stream.Position = 0; // Reset the stream position to the beginning
                return Result.Success((Stream)stream);
            }
            else
            {
                return Result.Failure<Stream>(Error.Failure("StorageService.DowloadAsync", "File not found"));
            }
        }
        catch (RequestFailedException ex)
        {
            return Result.Failure<Stream>(Error.Failure("StorageService.DowloadAsync", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<Stream>(Error.Failure("StorageService.DowloadAsync", ex.Message));
        }
    }
}
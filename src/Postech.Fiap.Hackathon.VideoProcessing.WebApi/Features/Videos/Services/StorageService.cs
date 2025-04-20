using Azure;
using Microsoft.Azure.Storage.Blob;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

public class StorageService(CloudBlobContainer container) : IStorageService
{
    public async Task<Result<string>> UploadVideoAsync(Guid videoId, Stream videoStream, string contentType,
        CancellationToken cancellationToken)
    {
        const string errorCode = "StorageService.UploadAsync";
        try
        {
            var blobName = $"{videoId}/{videoId}.mp4";
            var blob = container.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = contentType;

            await blob.UploadFromStreamAsync(videoStream, cancellationToken);

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

    public async Task<Result<Stream>> DownloadVideoAsync(string filePath, CancellationToken cancellationToken)
    {
        const string errorCode = "StorageService.DownloadVideoAsync";
        if (string.IsNullOrEmpty(filePath))
        {
            return Result.Failure<Stream>(Error.Failure(errorCode,
                "File path cannot be null or empty"));
        }

        try
        {
            var blob = container.GetBlockBlobReference(filePath);
            if (await blob.ExistsAsync(cancellationToken))
            {
                var stream = new MemoryStream();
                await blob.DownloadToStreamAsync(stream, cancellationToken);
                stream.Position = 0;
                return Result.Success<Stream>(stream);
            }
            else
            {
                return Result.Failure<Stream>(Error.Failure(errorCode, "File not found"));
            }
        }
        catch (RequestFailedException ex)
        {
            return Result.Failure<Stream>(Error.Failure(errorCode, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<Stream>(Error.Failure(errorCode, ex.Message));
        }
    }
}
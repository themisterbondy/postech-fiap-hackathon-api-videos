using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

public class VideoService(
    IVideoRepository videoRepository,
    IStorageService storageService) : IVideoService
{
    public async Task<Result<GetStatusVideoResponse>> getVideoById(Guid id, CancellationToken cancellationToken)
    {
        var video = await videoRepository.FindByIdAsync(id);
        if (video == null)
        {
            return Result.Failure<GetStatusVideoResponse>(Error.Failure("VideoService.getVideoById",
                "Video not found"));
        }

        var response = new GetStatusVideoResponse
        {
            Id = video.Id,
            Status = video.Status
        };
        return Result<GetStatusVideoResponse>.Success(response);
    }

    public async Task<Result<UploadVideoResponse>> upload(UploadVideoRequest request,
        CancellationToken cancellationToken)
    {
        Guid Id = Guid.NewGuid();

        var upload = await storageService.UploadAsync(Id, request.File.OpenReadStream(), request.File.ContentType);
        // chamar upload de video
        await videoRepository.AddAsync(new Video
        {
            Id = Id,
            Status = VideoStatus.Processing,
            FileName = request.File?.FileName ??
                       throw new ArgumentNullException(nameof(request.File), "File cannot be null"),
            FilePath = upload.Value
        });

        var response = new UploadVideoResponse
        {
            Id = Id,
            Status = VideoStatus.Processing
        };
        return Result.Success(response);
    }

    public async Task<Result<DownloadVideoZipResponse>> download(DownloadVideoZipRequest request,
        CancellationToken cancellationToken)
    {
        var video = await videoRepository.FindByIdAsync(request.Id);

        if (video == null)
        {
            return Result.Failure<DownloadVideoZipResponse>(Error.Failure("VideoService.DownloadVideoZipResponse",
                "Video not found"));
        }

        if (string.IsNullOrEmpty(video.FilePath))
        {
            return Result.Failure<DownloadVideoZipResponse>(Error.Failure("VideoService.DownloadVideoZipResponse",
                "File path is null or empty"));
        }

        var streamResult = await storageService.DowloadAsync(video.FilePath);

        if (!streamResult.IsSuccess)
        {
            return Result.Failure<DownloadVideoZipResponse>(Error.Failure("VideoService.DownloadVideoZipResponse",
                "Download with error"));
        }

        return new DownloadVideoZipResponse
        (
            streamResult.Value,
            "application/zip",
            $"{video.FileName}.zip"
        );
    }
}
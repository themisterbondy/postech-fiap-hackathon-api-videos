using Microsoft.AspNetCore.Identity;
using Namespace.Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Authentication.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

public class VideoService(
    IVideoRepository videoRepository,
    IStorageService storageService,
    IVideoQueueMessenger queueMessenger,
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor
) : IVideoService
{
    public async Task<Result<GetStatusVideoResponse>> GetVideoById(Guid videoId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);

        if (user is null)
            return Result.Failure<GetStatusVideoResponse>(Error.Failure("VideoService.UploadVideo",
                "User not found"));

        var userId = Guid.Parse(user.Id);

        var video = await videoRepository.FindByIdAsync(videoId, userId);

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

        return Result.Success(response);
    }

    public async Task<Result<UploadVideoResponse>> Upload(UploadVideoCommand.Command request,
        CancellationToken cancellationToken)
    {
        var videoId = Guid.NewGuid();
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);

        if (user is null)
            return Result.Failure<UploadVideoResponse>(Error.Failure("VideoService.UploadVideo",
                "User not found"));

        var userId = Guid.Parse(user.Id);

        var upload =
            await storageService.UploadVideoAsync(videoId, request.File!.OpenReadStream(), request.File.ContentType,
                cancellationToken);

        if (!upload.IsSuccess)
            return Result.Failure<UploadVideoResponse>(Error.Failure("VideoService.UploadVideo",
                "Upload with error"));

        var newVideo = new Video
        {
            Id = videoId,
            UserId = userId,
            Status = VideoStatus.Uploaded,
            FileName = request.File?.FileName,
            FilePath = upload.Value,
            ThumbnailsInterval = request.ThumbnailsInterval
        };

        await videoRepository.AddAsync(newVideo);

        var response = new UploadVideoResponse
        {
            Id = newVideo.Id,
            Status = newVideo.Status
        };

        await queueMessenger.SendAsync(videoId);

        return Result.Success(response);
    }

    public async Task<Result<DownloadVideoZipResponse>> Download(Guid videoId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User);
        var userId = Guid.Parse(user!.Id);

        var video = await videoRepository.FindByIdAsync(videoId, userId);

        if (video == null)
        {
            return Result.Failure<DownloadVideoZipResponse>(Error.Failure("VideoService.DownloadVideoZipResponse",
                "Video not found"));
        }

        if (string.IsNullOrEmpty(video.ThumbnailsZipPath))
        {
            return Result.Failure<DownloadVideoZipResponse>(Error.Failure("VideoService.DownloadVideoZipResponse",
                "File path is null or empty"));
        }

        var streamResult = await storageService.DownloadVideoAsync(video.ThumbnailsZipPath, cancellationToken);

        if (!streamResult.IsSuccess)
        {
            return Result.Failure<DownloadVideoZipResponse>(Error.Failure("VideoService.DownloadVideoZipResponse",
                "Download with error"));
        }

        return new DownloadVideoZipResponse
        (
            streamResult.Value,
            "application/x-zip-compressed",
            $"{videoId}.zip"
        );
    }
}
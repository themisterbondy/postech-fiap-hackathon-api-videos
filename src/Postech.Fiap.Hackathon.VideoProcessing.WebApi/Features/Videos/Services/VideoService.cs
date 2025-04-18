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
    public async Task<Result<GetStatusVideoResponse>> getVideoById(Guid videoId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

        if (user is null)
            return Result.Failure<GetStatusVideoResponse>(Error.Failure("VideoService.UploadVideo",
                "User not found"));

        var UserId = Guid.Parse(user.Id);

        var video = await videoRepository.FindByIdAsync(videoId, UserId);

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

    public async Task<Result<UploadVideoResponse>> upload(UploadVideoCreate.Command request,
        CancellationToken cancellationToken)
    {
        var VideoId = Guid.NewGuid();
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

        if (user is null)
            return Result.Failure<UploadVideoResponse>(Error.Failure("VideoService.UploadVideo",
                "User not found"));

        var UserId = Guid.Parse(user.Id);

        var upload =
            await storageService.UploadAsync(VideoId, request.File.OpenReadStream(), request.File.ContentType);

        if (!upload.IsSuccess)
            return Result.Failure<UploadVideoResponse>(Error.Failure("VideoService.UploadVideo",
                "Upload with error"));

        var newVideo = new Video
        {
            Id = VideoId,
            UserId = UserId,
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

        await queueMessenger.SendAsync(VideoId);

        return Result.Success(response);
    }

    public async Task<Result<DownloadVideoZipResponse>> download(Guid VideoId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        var UserId = Guid.Parse(user.Id);

        var video = await videoRepository.FindByIdAsync(VideoId, UserId);

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
            "application/x-zip-compressed",
            $"{VideoId}.zip"
        );
    }
}
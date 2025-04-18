using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;


public class VideoService(
    IVideoRepository videoRepository) : IVideoService {

    public async Task<Result<GetStatusVideoResponse>> getVideoById(Guid id, CancellationToken cancellationToken)
    {
        var video = await videoRepository.FindByIdAsync(id);
        if (video == null)
        {
            return Result.Failure<GetStatusVideoResponse>( Error.Failure("VideService.getVideoById", "Video not found"));
        }
        var response = new GetStatusVideoResponse
        {
            Id = video.Id,
            Status = video.Status
        };
        return Result<GetStatusVideoResponse>.Success(response);
    }
    
    public Task<Result<UploadVideoResponse>> upload(UploadVideoRequest request, CancellationToken cancellationToken)
    {
        // Implement the logic to upload a video
        throw new NotImplementedException();
    }
    
    public Task<Result<DownloadVideoZipResponse>> download(DownloadVideoZipRequest request, CancellationToken cancellationToken)
    {
        // Implement the logic to download a video as a zip
        throw new NotImplementedException();
    }



}

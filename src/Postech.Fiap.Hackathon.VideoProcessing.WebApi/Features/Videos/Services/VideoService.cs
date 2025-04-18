using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;


public class VideoService(
    IVideoRepository videoRepository) : IVideoService {

    public Task<Video?> getVideoById(Guid id, CancellationToken cancellationToken)
    {
        videoRepository.FindByIdAsync(id)
            .ContinueWith(task =>
            {
                if (task.Result == null)
                {
                    throw new Exception("Video not found");
                }
                return task.Result;
            }, cancellationToken);
    }

    public Task upload(UploadVideoRequest request, CancellationToken cancellationToken)
    {
        // Implement the logic to upload a video
        throw new NotImplementedException();
    }

    public Task download(DownloadVideoZipRequest request, CancellationToken cancellationToken)
    {
        // Implement the logic to download a video as a zip
        throw new NotImplementedException();
    }



}

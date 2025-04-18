
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class UploadVideoResponse
{
    public Guid Id { get; set; }
    public VideoStatus Status { get; set; }
    public UploadVideoResponse(Guid id, VideoStatus status)
    {
        Id = id;
        Status = status;
    }

}

using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class UploadVideoResponse
{
    public Guid id { get; set; }
    public VideoStatus status { get; set; }

}
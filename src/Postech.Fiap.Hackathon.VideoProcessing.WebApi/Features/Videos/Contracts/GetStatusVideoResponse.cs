using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Entities;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class GetStatusVideoResponse
{
    public Guid id { get; set; }
    public VideoStatus status { get; set; }

}
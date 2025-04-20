using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class GetStatusVideoResponse
{
    public Guid Id { get; set; }
    public VideoStatus Status { get; set; }

}
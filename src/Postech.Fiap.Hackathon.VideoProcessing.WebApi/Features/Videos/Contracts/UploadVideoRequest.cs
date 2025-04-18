namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class UploadVideoRequest
{
    public int ThumbnailsInterval { get; set; }
    public IFormFile? File { get; set; }
}
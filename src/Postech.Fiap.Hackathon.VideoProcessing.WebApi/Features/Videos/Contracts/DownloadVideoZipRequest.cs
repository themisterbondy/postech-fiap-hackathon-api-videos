namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class DownloadVideoZipRequest
{
    public Guid Id { get; set; }
    public DownloadVideoZipRequest(Guid Id)
    {
        this.Id = Id;
    }

}
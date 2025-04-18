namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class DownloadVideoZipResponse
{
    public Stream File { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

public class DownloadVideoZipResponse(Stream file, string contentType, string fileName)
{
    public Stream File { get; set; } = file;
    public string ContentType { get; set; } = contentType;
    public string FileName { get; set; } = fileName;
}
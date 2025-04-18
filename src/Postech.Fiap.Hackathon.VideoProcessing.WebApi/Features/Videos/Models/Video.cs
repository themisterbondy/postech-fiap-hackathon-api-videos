namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

public class Video
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; } = string.Empty;
    public int ThumbnailsInterval { get; set; }
    public string? ThumbnailsZipPath { get; set; } = string.Empty;
    public VideoStatus Status { get; set; } = VideoStatus.Processing;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;

    public Video(Guid id, Guid userId, string fileName, string filePath, int thumbnailsInterval, string? thumbnailsZipPath, VideoStatus status, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        FileName = fileName;
        FilePath = filePath;
        ThumbnailsInterval = thumbnailsInterval;
        ThumbnailsZipPath = thumbnailsZipPath;
        Status = status;
        CreatedAt = createdAt;
    }

    public Video()
    {
    }
    public static Video Create(Guid Id, int ThumbnailsInterval)
    {
        return new Video()
        {
            Id = Id,
            ThumbnailsInterval = ThumbnailsInterval,
        };
    }
}
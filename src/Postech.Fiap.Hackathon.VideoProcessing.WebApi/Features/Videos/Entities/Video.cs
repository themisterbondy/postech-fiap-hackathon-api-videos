using System;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Entities
{
    public class Video
    {

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int ThumbnailsInterval { get; set; }
        public string? ThumbnailsZipPath { get; set; } = string.Empty;
        public VideoStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        private Video()
        {
        }
        public Video(Guid userId, string fileName, string filePath, int thumbnailsInterval)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            FileName = fileName;
            FilePath = filePath;
            ThumbnailsInterval = thumbnailsInterval;
            ThumbnailsZipPath = string.Empty;
            Status = VideoStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
    }

    public enum VideoStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }
}
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Models;

public class VideoTests
{
    [Fact(DisplayName = "Deve criar objeto corretamente com construtor completo")]
    public void Constructor_WithAllProperties_Should_Set_Properties_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var fileName = "video.mp4";
        var filePath = "/path/to/video.mp4";
        var thumbnailsInterval = 10;
        var zipPath = "/path/to/thumbs.zip";
        var status = VideoStatus.Processed;
        var createdAt = DateTime.UtcNow;

        // Act
        var video = new Video(id, userId, fileName, filePath, thumbnailsInterval, zipPath, status, createdAt);

        // Assert
        video.Id.Should().Be(id);
        video.UserId.Should().Be(userId);
        video.FileName.Should().Be(fileName);
        video.FilePath.Should().Be(filePath);
        video.ThumbnailsInterval.Should().Be(thumbnailsInterval);
        video.ThumbnailsZipPath.Should().Be(zipPath);
        video.Status.Should().Be(status);
        video.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromSeconds(1));
    }

    [Fact(DisplayName = "Deve permitir set e get manualmente com construtor padrão")]
    public void DefaultConstructor_Should_Allow_Setting_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var video = new Video
        {
            Id = id,
            UserId = userId,
            FileName = "test.mp4",
            FilePath = "/files/test.mp4",
            ThumbnailsInterval = 5,
            ThumbnailsZipPath = "/zip/thumbs.zip",
            Status = VideoStatus.Uploaded,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        video.Id.Should().Be(id);
        video.UserId.Should().Be(userId);
        video.FileName.Should().Be("test.mp4");
        video.FilePath.Should().Be("/files/test.mp4");
        video.ThumbnailsInterval.Should().Be(5);
        video.ThumbnailsZipPath.Should().Be("/zip/thumbs.zip");
        video.Status.Should().Be(VideoStatus.Uploaded);
    }

    [Fact(DisplayName = "Create deve instanciar com ID e ThumbnailsInterval corretos")]
    public void Create_Should_Set_Id_And_ThumbnailsInterval()
    {
        // Arrange
        var id = Guid.NewGuid();
        var interval = 15;

        // Act
        var video = Video.Create(id, interval);

        // Assert
        video.Id.Should().Be(id);
        video.ThumbnailsInterval.Should().Be(interval);
        video.Status.Should().Be(VideoStatus.Processing); // default
        video.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
using Microsoft.EntityFrameworkCore;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Persistence;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Repositories;

public class VideoRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public VideoRepositoryTests()
    {
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // isolamento entre testes
            .Options;
    }

    [Fact(DisplayName = "AddAsync deve salvar vídeo no banco de dados")]
    public async Task AddAsync_Should_SaveVideoToDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbOptions);
        var repository = new VideoRepository(context);

        var video = new Video
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FileName = "video.mp4",
            FilePath = "/files/video.mp4",
            ThumbnailsInterval = 10,
            Status = VideoStatus.Uploaded,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(video);

        // Assert
        var exists = await context.Videos.AnyAsync(v => v.Id == video.Id);
        exists.Should().BeTrue();
    }

    [Fact(DisplayName = "FindByIdAsync deve retornar vídeo correto se existir")]
    public async Task FindByIdAsync_Should_ReturnVideo_When_Exists()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var video = new Video
        {
            Id = videoId,
            UserId = userId,
            FileName = "video.mp4",
            FilePath = "/files/video.mp4",
            ThumbnailsInterval = 10,
            Status = VideoStatus.Processed,
            CreatedAt = DateTime.UtcNow
        };

        using (var context = new ApplicationDbContext(_dbOptions))
        {
            context.Videos.Add(video);
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(_dbOptions))
        {
            var repository = new VideoRepository(context);

            // Act
            var result = await repository.FindByIdAsync(videoId, userId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(videoId);
            result.UserId.Should().Be(userId);
        }
    }

    [Fact(DisplayName = "FindByIdAsync deve retornar null se vídeo não existir")]
    public async Task FindByIdAsync_Should_ReturnNull_When_NotFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbOptions);
        var repository = new VideoRepository(context);

        // Act
        var result = await repository.FindByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
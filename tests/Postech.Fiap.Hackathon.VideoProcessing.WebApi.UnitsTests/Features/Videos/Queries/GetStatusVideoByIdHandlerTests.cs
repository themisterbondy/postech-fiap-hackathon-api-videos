using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queries;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Queries;

public class GetStatusVideoByIdHandlerTests
{
    private readonly GetStatusVideoById.Handler _handler;
    private readonly IVideoService _videoService = Substitute.For<IVideoService>();

    public GetStatusVideoByIdHandlerTests()
    {
        _handler = new GetStatusVideoById.Handler(_videoService);
    }

    [Fact(DisplayName = "Handle deve retornar sucesso quando vídeo é encontrado")]
    public async Task Handle_Should_ReturnSuccess_When_VideoExists()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var expected = new GetStatusVideoResponse
        {
            Id = videoId,
            Status = VideoStatus.Processed
        };

        _videoService.GetVideoById(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(expected));

        var query = new GetStatusVideoById.Query { Id = videoId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact(DisplayName = "Handle deve retornar falha quando vídeo não é encontrado")]
    public async Task Handle_Should_ReturnFailure_When_VideoNotFound()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var error = Error.NotFound("VID404", "Video not found");

        _videoService.GetVideoById(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<GetStatusVideoResponse>(error));

        var query = new GetStatusVideoById.Query { Id = videoId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VID404");
        result.Error.Message.Should().Be("Video not found");
    }
}
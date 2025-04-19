using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Command;

public class DownloadVideoCommandHandlerTests
{
    private readonly DownloadVideoCommand.DownloadVideoCommandHandler _handler;
    private readonly IVideoService _videoServiceMock = Substitute.For<IVideoService>();

    public DownloadVideoCommandHandlerTests()
    {
        _handler = new DownloadVideoCommand.DownloadVideoCommandHandler(_videoServiceMock);
    }

    [Fact(DisplayName = "Handle deve retornar falha quando o serviço retorna falha")]
    public async Task Handle_Should_ReturnFailure_When_ServiceFails()
    {
        // Arrange
        var command = new DownloadVideoCommand.Command { Id = Guid.NewGuid() };
        var error = Error.Failure("DOWNLOAD_ERROR", "Erro ao baixar vídeo");

        _videoServiceMock.Download(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<DownloadVideoZipResponse>(error));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DOWNLOAD_ERROR");
        result.Error.Message.Should().Contain("Erro ao baixar vídeo");
    }

    [Fact(DisplayName = "Handle deve retornar sucesso quando o serviço retorna sucesso")]
    public async Task Handle_Should_ReturnSuccess_When_ServiceSucceeds()
    {
        // Arrange
        var command = new DownloadVideoCommand.Command { Id = Guid.NewGuid() };

        var stream = new MemoryStream();
        var expectedResponse = new DownloadVideoZipResponse(stream, "application/zip", "video.zip");

        _videoServiceMock.Download(command.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Success(expectedResponse));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.File.Should().BeSameAs(stream);
        result.Value.ContentType.Should().Be("application/zip");
        result.Value.FileName.Should().Be("video.zip");
    }
}
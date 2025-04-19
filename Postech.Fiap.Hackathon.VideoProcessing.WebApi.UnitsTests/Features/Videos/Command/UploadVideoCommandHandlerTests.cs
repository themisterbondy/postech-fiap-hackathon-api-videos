using Microsoft.AspNetCore.Http;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Command;

public class UploadVideoCommandHandlerTests
{
    private readonly UploadVideoCommand.UploadVideoCommandHandler _handler;
    private readonly IVideoService _videoServiceMock = Substitute.For<IVideoService>();

    public UploadVideoCommandHandlerTests()
    {
        _handler = new UploadVideoCommand.UploadVideoCommandHandler(_videoServiceMock);
    }

    [Fact(DisplayName = "Handle deve retornar sucesso quando o serviço retorna sucesso")]
    public async Task Handle_Should_ReturnSuccess_When_ServiceReturnsSuccess()
    {
        // Arrange
        var request = new UploadVideoCommand.Command
        {
            ThumbnailsInterval = 10,
            File = Substitute.For<IFormFile>()
        };

        var response = new UploadVideoResponse
        {
            Id = Guid.NewGuid(),
            Status = VideoStatus.Uploaded
        };

        _videoServiceMock.Upload(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact(DisplayName = "Handle deve retornar falha quando o serviço retorna falha")]
    public async Task Handle_Should_ReturnFailure_When_ServiceReturnsFailure()
    {
        // Arrange
        var request = new UploadVideoCommand.Command
        {
            ThumbnailsInterval = 5,
            File = Substitute.For<IFormFile>()
        };

        var error = Error.Failure("UPLOAD_ERR", "Erro ao fazer upload");

        _videoServiceMock.Upload(request, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<UploadVideoResponse>(error));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue(result.Error.Message);
        result.Error.Code.Should().Be("UPLOAD_ERR");
        result.Error.Message.Should().Contain("Erro ao fazer upload");
    }
}
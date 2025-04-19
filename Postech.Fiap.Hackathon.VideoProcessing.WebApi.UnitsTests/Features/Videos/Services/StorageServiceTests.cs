using Microsoft.Azure.Storage.Blob;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Services;

public class StorageServiceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CloudBlobContainer _containerMock;
    private readonly StorageService _sut;

    public StorageServiceTests()
    {
        // Usando construtor com URI explícito para evitar erro do Castle Proxy
        _containerMock = Substitute.For<CloudBlobContainer>(new Uri("https://test.blob.core.windows.net/container"));
        _sut = new StorageService(_containerMock);
    }

    [Fact(DisplayName = "UploadVideoAsync deve retornar sucesso quando upload for bem-sucedido")]
    public async Task UploadVideoAsync_Should_ReturnSuccess_When_UploadSucceeds()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var stream = new MemoryStream();
        var blobName = $"{videoId}/{videoId}.mp4";

        var blobMock =
            Substitute.For<CloudBlockBlob>(new Uri($"https://test.blob.core.windows.net/container/{blobName}"));
        _containerMock.GetBlockBlobReference(blobName).Returns(blobMock);

        // Act
        var result = await _sut.UploadVideoAsync(videoId, stream, "video/mp4", _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(blobName);
        await blobMock.Received(1).UploadFromStreamAsync(stream, _cancellationToken);
        blobMock.Properties.ContentType.Should().Be("video/mp4");
    }

    [Fact(DisplayName = "UploadVideoAsync deve retornar falha quando uma exceção inesperada for lançada")]
    public async Task UploadVideoAsync_Should_ReturnFailure_When_ExceptionThrown()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var stream = new MemoryStream();
        var blobName = $"{videoId}/{videoId}.mp4";

        var blobMock =
            Substitute.For<CloudBlockBlob>(new Uri($"https://test.blob.core.windows.net/container/{blobName}"));
        blobMock
            .When(x => x.UploadFromStreamAsync(stream, _cancellationToken))
            .Do(_ => throw new Exception("Erro inesperado"));

        _containerMock.GetBlockBlobReference(blobName).Returns(blobMock);

        // Act
        var result = await _sut.UploadVideoAsync(videoId, stream, "video/mp4", _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Erro inesperado");
        result.Error.Code.Should().Be("StorageService.UploadAsync");
    }

    [Fact(DisplayName = "DownloadVideoAsync deve retornar falha se o caminho estiver vazio")]
    public async Task DownloadVideoAsync_Should_ReturnFailure_When_FilePathIsEmpty()
    {
        // Act
        var result = await _sut.DownloadVideoAsync("", _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("cannot be null or empty");
    }

    [Fact(DisplayName = "DownloadVideoAsync deve retornar falha se blob não existir")]
    public async Task DownloadVideoAsync_Should_ReturnFailure_When_BlobDoesNotExist()
    {
        // Arrange
        var filePath = "videos/test.mp4";
        var blobMock =
            Substitute.For<CloudBlockBlob>(new Uri($"https://test.blob.core.windows.net/container/{filePath}"));
        _containerMock.GetBlockBlobReference(filePath).Returns(blobMock);
        blobMock.ExistsAsync(_cancellationToken).Returns(false);

        // Act
        var result = await _sut.DownloadVideoAsync(filePath, _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("File not found");
    }

    [Fact(DisplayName = "DownloadVideoAsync deve retornar sucesso quando blob é encontrado e baixado")]
    public async Task DownloadVideoAsync_Should_ReturnSuccess_When_BlobExists()
    {
        // Arrange
        var filePath = "videos/test.mp4";
        var blobMock =
            Substitute.For<CloudBlockBlob>(new Uri($"https://test.blob.core.windows.net/container/{filePath}"));
        _containerMock.GetBlockBlobReference(filePath).Returns(blobMock);
        blobMock.ExistsAsync(_cancellationToken).Returns(true);

        blobMock
            .When(b => b.DownloadToStreamAsync(Arg.Any<Stream>(), _cancellationToken))
            .Do(call =>
            {
                var stream = call.Arg<Stream>();
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write("conteúdo de teste");
                writer.Flush();
                stream.Position = 0;
            });

        // Act
        var result = await _sut.DownloadVideoAsync(filePath, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        using var reader = new StreamReader(result.Value);
        var content = await reader.ReadToEndAsync(_cancellationToken);
        content.Should().Contain("conteúdo de teste");
    }

    [Fact(DisplayName = "DownloadVideoAsync deve retornar falha se exceção for lançada")]
    public async Task DownloadVideoAsync_Should_ReturnFailure_When_ExceptionThrown()
    {
        // Arrange
        var filePath = "videos/test.mp4";
        var blobMock =
            Substitute.For<CloudBlockBlob>(new Uri($"https://test.blob.core.windows.net/container/{filePath}"));

        blobMock.When(b => b.ExistsAsync(_cancellationToken)).Do(_ => throw new Exception("falha inesperada"));
        _containerMock.GetBlockBlobReference(filePath).Returns(blobMock);

        // Act
        var result = await _sut.DownloadVideoAsync(filePath, _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("falha inesperada");
        result.Error.Code.Should().Be("StorageService.DownloadVideoAsync");
    }
}
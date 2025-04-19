using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Contracts;

public class DownloadVideoZipResponseTests
{
    [Fact(DisplayName = "Deve permitir set e get de File, ContentType e FileName")]
    public void Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var stream = new MemoryStream();
        var contentType = "application/zip";
        var fileName = "video123.zip";

        // Act
        var response = new DownloadVideoZipResponse(stream, contentType, fileName);

        // Assert
        response.File.Should().BeSameAs(stream);
        response.ContentType.Should().Be(contentType);
        response.FileName.Should().Be(fileName);
    }

    [Fact(DisplayName = "Propriedades devem poder ser alteradas após instanciado")]
    public void Properties_Should_Be_Settable_After_Creation()
    {
        // Arrange
        var original = new DownloadVideoZipResponse(Stream.Null, "application/octet-stream", "old.zip");

        var newStream = new MemoryStream();
        var newContentType = "application/x-zip-compressed";
        var newFileName = "newfile.zip";

        // Act
        original.File = newStream;
        original.ContentType = newContentType;
        original.FileName = newFileName;

        // Assert
        original.File.Should().BeSameAs(newStream);
        original.ContentType.Should().Be(newContentType);
        original.FileName.Should().Be(newFileName);
    }
}
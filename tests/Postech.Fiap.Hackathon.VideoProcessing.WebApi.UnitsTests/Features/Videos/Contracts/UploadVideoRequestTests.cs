using Microsoft.AspNetCore.Http;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Contracts;

public class UploadVideoRequestTests
{
    [Fact(DisplayName = "Deve permitir set e get de ThumbnailsInterval e File")]
    public void Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var interval = 10;
        var mockFile = Substitute.For<IFormFile>();

        // Act
        var request = new UploadVideoRequest
        {
            ThumbnailsInterval = interval,
            File = mockFile
        };

        // Assert
        request.ThumbnailsInterval.Should().Be(interval);
        request.File.Should().Be(mockFile);
    }
}
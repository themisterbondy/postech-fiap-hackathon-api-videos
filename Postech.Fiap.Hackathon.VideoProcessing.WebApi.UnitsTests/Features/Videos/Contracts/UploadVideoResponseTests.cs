using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Contracts;

public class UploadVideoResponseTests
{
    [Fact(DisplayName = "Deve permitir set e get de Id e Status")]
    public void Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = VideoStatus.Processing;

        // Act
        var response = new UploadVideoResponse
        {
            Id = id,
            Status = status
        };

        // Assert
        response.Id.Should().Be(id);
        response.Status.Should().Be(status);
    }
}
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Contracts;

public class GetStatusVideoResponseTests
{
    [Fact(DisplayName = "GetStatusVideoResponse deve permitir set e get de Id e Status")]
    public void GetStatusVideoResponse_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = VideoStatus.Processed;

        var response = new GetStatusVideoResponse
        {
            Id = id,
            Status = status
        };

        // Assert
        response.Id.Should().Be(id);
        response.Status.Should().Be(status);
    }
}
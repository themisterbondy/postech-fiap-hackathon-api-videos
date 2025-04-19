using Microsoft.Azure.Storage.Queue;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queue;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Queue;

public class VideoQueueMessengerTests
{
    [Fact(DisplayName = "SendAsync deve enviar mensagem com o ID do vídeo")]
    public async Task SendAsync_Should_Send_Message_With_VideoId()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var cloudQueue = Substitute.For<CloudQueue>(new Uri("https://fakestorage.queue.core.windows.net/test-queue"));
        var sut = new VideoQueueMessenger(cloudQueue);

        // Act
        await sut.SendAsync(videoId);

        // Assert
        await cloudQueue.Received(1).AddMessageAsync(
            Arg.Is<CloudQueueMessage>(msg => msg.AsString == videoId.ToString()));
    }
}
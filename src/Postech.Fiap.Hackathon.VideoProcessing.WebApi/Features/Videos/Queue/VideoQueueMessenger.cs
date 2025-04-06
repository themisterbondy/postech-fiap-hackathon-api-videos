using Microsoft.Azure.Storage.Queue;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queue;

public class VideoQueueMessenger(CloudQueue queue) : IVideoQueueMessenger
{
    public async Task SendAsync(Guid VideoId)
    {
        var message = new CloudQueueMessage(VideoId.ToString());
        await queue.AddMessageAsync(message);
    }
}
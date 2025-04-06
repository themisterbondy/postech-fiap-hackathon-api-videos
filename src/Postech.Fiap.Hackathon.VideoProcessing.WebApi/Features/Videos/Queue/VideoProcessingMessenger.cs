using FluentStorage.Messaging;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queue;

public class VideoProcessingMessenger(IMessenger messenger) : IVideoProcessingMessenger
{
    private const string Channel = "video-processing";

    public Task SendAsync(Guid VideoId)
    {
        messenger.CreateChannelAsync(Channel);
        return messenger.SendAsync(Channel, new QueueMessage(VideoId.ToString()));
    }
}
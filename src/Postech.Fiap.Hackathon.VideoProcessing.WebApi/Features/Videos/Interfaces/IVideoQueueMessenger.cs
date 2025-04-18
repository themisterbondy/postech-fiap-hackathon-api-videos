namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

public interface IVideoQueueMessenger
{
    Task SendAsync(Guid VideoId);
}
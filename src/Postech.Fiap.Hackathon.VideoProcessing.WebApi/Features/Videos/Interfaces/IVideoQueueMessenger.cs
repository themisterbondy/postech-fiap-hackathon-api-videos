namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

internal interface IVideoQueueMessenger
{
    Task SendAsync(Guid VideoId);
}
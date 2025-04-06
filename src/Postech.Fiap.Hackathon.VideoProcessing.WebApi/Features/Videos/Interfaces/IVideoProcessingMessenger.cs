namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

internal interface IVideoProcessingMessenger
{
    Task SendAsync(Guid VideoId);
}
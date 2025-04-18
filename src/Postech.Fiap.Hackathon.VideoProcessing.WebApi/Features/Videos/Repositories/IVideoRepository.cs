using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

public interface IVideoRepository
{
    Task<Video?> FindByIdAsync(Guid id);
    Task AddAsync(Video video);
}

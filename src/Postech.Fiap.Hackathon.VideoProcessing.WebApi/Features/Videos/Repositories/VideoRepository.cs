using Microsoft.EntityFrameworkCore;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Persistence;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

public class VideoRepository(ApplicationDbContext context) : IVideoRepository
{
    public async Task<Video?> FindByIdAsync(Guid videoId, Guid userId)
    {
        return await context.Videos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == videoId && c.UserId == userId);
    }

    public async Task AddAsync(Video video)
    {
        await context.Videos.AddAsync(video);
        await context.SaveChangesAsync();
    }
}
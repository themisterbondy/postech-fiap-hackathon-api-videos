using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Persistence;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;


public class VideoRepository(ApplicationDbContext context) : IVideoRepository
{

    public async Task<Video?> FindByIdAsync(Guid id)
    {
        return await context.Videos.AsNoTracking().Include(c => c.Status).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Video video)
    {
        await context.Videos.AddAsync(video);
        await context.SaveChangesAsync();
    }
}
namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;

using Microsoft.EntityFrameworkCore;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Entities;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Persistence;

public class VideoRepository(ApplicationDbContext context) : IVideoRepository
{

    public async Task<Video?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Videos
            .AsNoTracking()
            .Include(o => o.UserId)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        
    }

    public async Task AddAsync(Video video, CancellationToken cancellationToken)
    {
        await context.AddAsync(video, cancellationToken);
    }

    public async Task UpdateAsync(Video video, CancellationToken cancellationToken)
    {
        context.Update(video);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var video = await GetByIdAsync(id, cancellationToken);
        if (video != null)
        {
            context.Remove(video);
        }
    }
}
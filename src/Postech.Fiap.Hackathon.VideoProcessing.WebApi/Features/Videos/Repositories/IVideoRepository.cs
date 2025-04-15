namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;
using Microsoft.EntityFrameworkCore;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Entities;

public interface IVideoRepository
{
    Task<Result<Video>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Video>> AddAsync(Video video, CancellationToken cancellationToken);
    Task<Result<Video>> UpdateAsync(Video video, CancellationToken cancellationToken);
    Task<Result<Video>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

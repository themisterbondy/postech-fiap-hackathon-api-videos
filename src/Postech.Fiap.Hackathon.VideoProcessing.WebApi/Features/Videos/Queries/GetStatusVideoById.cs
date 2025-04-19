using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queries;

public abstract class GetStatusVideoById
{
    public class Query : IRequest<Result<GetStatusVideoResponse>>
    {
        public Guid Id { get; init; }
    }

    public class Handler(IVideoService iVideoService) : IRequestHandler<Query, Result<GetStatusVideoResponse>>
    {
        public async Task<Result<GetStatusVideoResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await iVideoService.GetVideoById(request.Id, cancellationToken);
        }
    }
}
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Contracts;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Queries;

public class GetStatusVideoById
{
    public class Query : IRequest<Result<GetStatusVideoResponse>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IVideoService iVideoService) : IRequestHandler<Query, Result<GetStatusVideoResponse>>
    {
        public async Task<Result<GetStatusVideoResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await iVideoService.getVideoById(request.Id, cancellationToken);
        }
    }
}
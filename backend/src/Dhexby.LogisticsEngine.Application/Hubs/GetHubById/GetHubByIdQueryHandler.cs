using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Hubs.GetHubById;

/// <summary>Use case di lettura: proiezione DTO dell'hub.</summary>
public sealed class GetHubByIdQueryHandler : IRequestHandler<GetHubByIdQuery, Result<HubResponse>>
{
    private readonly IHubRepository _hubRepository;

    public GetHubByIdQueryHandler(IHubRepository hubRepository)
    {
        _hubRepository = hubRepository;
    }

    public async Task<Result<HubResponse>> Handle(GetHubByIdQuery request, CancellationToken cancellationToken)
    {
        Hub? hub = await _hubRepository.GetByIdAsync(request.HubId, cancellationToken);

        if (hub is null)
            return Result.Failure<HubResponse>(HubErrors.NotFound(request.HubId));

        return HubResponse.FromDomain(hub);
    }
}

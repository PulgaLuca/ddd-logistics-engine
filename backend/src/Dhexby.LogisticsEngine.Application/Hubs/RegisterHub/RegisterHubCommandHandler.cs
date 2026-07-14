using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;

/// <summary>Use case: registra un nuovo hub nella rete (stato iniziale Active).</summary>
public sealed class RegisterHubCommandHandler : IRequestHandler<RegisterHubCommand, Result<Guid>>
{
    private readonly IHubRepository _hubRepository;

    public RegisterHubCommandHandler(IHubRepository hubRepository)
    {
        _hubRepository = hubRepository;
    }

    public async Task<Result<Guid>> Handle(RegisterHubCommand request, CancellationToken cancellationToken)
    {
        Result<HubCode> codeResult = HubCode.Create(request.Code);
        if (codeResult.IsFailure)
        {
            return Result.Failure<Guid>(codeResult.Error);
        }

        Result<GeoCoordinate> coordinateResult = GeoCoordinate.Create(request.Latitude, request.Longitude);
        if (coordinateResult.IsFailure)
        {
            return Result.Failure<Guid>(coordinateResult.Error);
        }

        bool codeInUse = await _hubRepository.ExistsByCodeAsync(codeResult.Value, cancellationToken);
        if (codeInUse)
        {
            return Result.Failure<Guid>(HubErrors.CodeAlreadyInUse);
        }

        Result<Hub> hubResult = Hub.Create(codeResult.Value, request.Name, coordinateResult.Value, DateTime.UtcNow);
        if (hubResult.IsFailure)
        {
            return Result.Failure<Guid>(hubResult.Error);
        }

        await _hubRepository.AddAsync(hubResult.Value, cancellationToken);

        return hubResult.Value.Id;
    }
}

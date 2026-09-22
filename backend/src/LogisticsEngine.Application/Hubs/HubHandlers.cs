using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs;

public sealed class GetHubByIdHandler(IHubRepository repository) : IRequestHandler<GetHubById, Result<HubDto>>
{
    public async Task<Result<HubDto>> Handle(GetHubById request, CancellationToken cancellationToken)
    {
        Hub? hub = await repository.GetByIdAsync(request.Id, cancellationToken);
        return hub is null ? Result.Failure<HubDto>(HubErrors.NotFound(request.Id)) : Result.Success(HubDto.From(hub));
    }
}

public sealed class GetAllHubsHandler(IHubRepository repository) : IRequestHandler<GetAllHubs, IReadOnlyList<HubDto>>
{
    public async Task<IReadOnlyList<HubDto>> Handle(GetAllHubs request, CancellationToken cancellationToken) =>
        (await repository.GetAllAsync(cancellationToken)).Select(HubDto.From).ToArray();
}

public sealed class CreateHubHandler(IHubRepository repository) : IRequestHandler<CreateHub, Result<HubDto>>
{
    public async Task<Result<HubDto>> Handle(CreateHub request, CancellationToken cancellationToken)
    {
        Result<Hub> result = Hub.Create(Guid.NewGuid(), request.Name, request.Zone);
        if (result.IsFailure)
        {
            return Result.Failure<HubDto>(result.Error);
        }

        await repository.AddAsync(result.Value, cancellationToken);
        return Result.Success(HubDto.From(result.Value));
    }
}

public sealed class UpdateHubHandler(IHubRepository repository) : IRequestHandler<UpdateHub, Result>
{
    public async Task<Result> Handle(UpdateHub request, CancellationToken cancellationToken)
    {
        Hub? hub = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (hub is null)
        {
            return Result.Failure(HubErrors.NotFound(request.Id));
        }

        Result result = hub.Update(request.Name, request.Zone);
        if (result.IsFailure)
        {
            return result;
        }

        return await repository.UpdateAsync(hub, cancellationToken)
            ? Result.Success()
            : Result.Failure(HubErrors.NotFound(request.Id));
    }
}

public sealed class DeleteHubHandler(IHubRepository repository) : IRequestHandler<DeleteHub, Result>
{
    public async Task<Result> Handle(DeleteHub request, CancellationToken cancellationToken) =>
        await repository.DeleteAsync(request.Id, cancellationToken)
            ? Result.Success()
            : Result.Failure(HubErrors.NotFound(request.Id));
}

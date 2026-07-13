using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.AssignOrder;

/// <summary>
/// Use case: assegna un ordine a un hub. Verifica l'esistenza dell'hub
/// (coordinamento fra aggregati Order e Hub) prima di applicare la regola.
/// </summary>
public sealed class AssignOrderCommandHandler : IRequestHandler<AssignOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IHubRepository _hubRepository;

    public AssignOrderCommandHandler(IOrderRepository orderRepository, IHubRepository hubRepository)
    {
        _orderRepository = orderRepository;
        _hubRepository = hubRepository;
    }

    public async Task<Result> Handle(AssignOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(request.OrderId));
        }

        Hub? hub = await _hubRepository.GetByIdAsync(request.HubId, cancellationToken);
        if (hub is null)
        {
            return Result.Failure(HubErrors.NotFound(request.HubId));
        }

        Result assignResult = order.AssignToHub(hub.Id);
        if (assignResult.IsFailure)
        {
            return assignResult;
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return Result.Success();
    }
}

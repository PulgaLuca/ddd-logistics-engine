using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.RedoDispatch;

/// <summary>
/// Use case: redo dell'assegnazione. Ri-applica l'ultima azione annullata
/// (riassegna l'ordine allo stesso hub) e la riporta sulla pila di undo.
/// </summary>
public sealed class RedoDispatchCommandHandler
    : IRequestHandler<RedoDispatchCommand, Result<DispatchHistoryResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDispatchHistory _dispatchHistory;

    public RedoDispatchCommandHandler(IOrderRepository orderRepository, IDispatchHistory dispatchHistory)
    {
        _orderRepository = orderRepository;
        _dispatchHistory = dispatchHistory;
    }

    public async Task<Result<DispatchHistoryResponse>> Handle(
        RedoDispatchCommand request,
        CancellationToken cancellationToken)
    {
        DispatchAction? action = _dispatchHistory.PeekRedo();
        if (action is null)
        {
            return Result.Failure<DispatchHistoryResponse>(DispatchErrors.NothingToRedo);
        }

        Order? order = await _orderRepository.GetByIdAsync(action.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<DispatchHistoryResponse>(OrderErrors.NotFound(action.OrderId));
        }

        Result assignResult = order.AssignToHub(action.HubId);
        if (assignResult.IsFailure)
        {
            return Result.Failure<DispatchHistoryResponse>(assignResult.Error);
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        _dispatchHistory.CommitRedo();

        return DispatchHistoryResponse.From(_dispatchHistory);
    }
}

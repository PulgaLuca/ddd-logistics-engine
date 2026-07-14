using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.UndoDispatch;

/// <summary>
/// Use case: undo dell'assegnazione. Sfila l'ultima azione dalla pila di undo,
/// riporta l'ordine a Pending (Unassign) e sposta l'azione sulla pila di redo.
/// L'ordine di operazioni garantisce che la cronologia cambi solo se anche il
/// dominio accetta la modifica.
/// </summary>
public sealed class UndoDispatchCommandHandler
    : IRequestHandler<UndoDispatchCommand, Result<DispatchHistoryResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDispatchHistory _dispatchHistory;

    public UndoDispatchCommandHandler(IOrderRepository orderRepository, IDispatchHistory dispatchHistory)
    {
        _orderRepository = orderRepository;
        _dispatchHistory = dispatchHistory;
    }

    public async Task<Result<DispatchHistoryResponse>> Handle(
        UndoDispatchCommand request,
        CancellationToken cancellationToken)
    {
        DispatchAction? action = _dispatchHistory.PeekUndo();
        if (action is null)
        {
            return Result.Failure<DispatchHistoryResponse>(DispatchErrors.NothingToUndo);
        }

        Order? order = await _orderRepository.GetByIdAsync(action.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<DispatchHistoryResponse>(OrderErrors.NotFound(action.OrderId));
        }

        Result unassignResult = order.Unassign();
        if (unassignResult.IsFailure)
        {
            return Result.Failure<DispatchHistoryResponse>(unassignResult.Error);
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        _dispatchHistory.CommitUndo();

        return DispatchHistoryResponse.From(_dispatchHistory);
    }
}

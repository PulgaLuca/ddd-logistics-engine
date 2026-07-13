using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Processing.GetExpressDispatchQueue;

/// <summary>Use case: costruisce la coda a priorità di dispatch dagli ordini Pending.</summary>
public sealed class GetExpressDispatchQueueQueryHandler
    : IRequestHandler<GetExpressDispatchQueueQuery, Result<OrderQueueResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDispatchPriorityQueue _priorityQueue;

    public GetExpressDispatchQueueQueryHandler(
        IOrderRepository orderRepository,
        IOrderDispatchPriorityQueue priorityQueue)
    {
        _orderRepository = orderRepository;
        _priorityQueue = priorityQueue;
    }

    public async Task<Result<OrderQueueResponse>> Handle(
        GetExpressDispatchQueueQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Order> pending = await _orderRepository.GetPendingAsync(cancellationToken);
        IReadOnlyList<Order> arranged = _priorityQueue.Arrange(pending);

        return OrderQueueResponse.From("Priority", arranged);
    }
}

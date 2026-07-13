using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Processing.GetOrderProcessingQueue;

/// <summary>Use case: costruisce la coda FIFO di processamento dagli ordini Pending.</summary>
public sealed class GetOrderProcessingQueueQueryHandler
    : IRequestHandler<GetOrderProcessingQueueQuery, Result<OrderQueueResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderProcessingQueue _processingQueue;

    public GetOrderProcessingQueueQueryHandler(
        IOrderRepository orderRepository,
        IOrderProcessingQueue processingQueue)
    {
        _orderRepository = orderRepository;
        _processingQueue = processingQueue;
    }

    public async Task<Result<OrderQueueResponse>> Handle(
        GetOrderProcessingQueueQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Order> pending = await _orderRepository.GetPendingAsync(cancellationToken);
        IReadOnlyList<Order> arranged = _processingQueue.Arrange(pending);

        return OrderQueueResponse.From("Fifo", arranged);
    }
}

using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Processing.GetOrderProcessingQueue;

/// <summary>Query: ordini in attesa nell'ordine di lavorazione FIFO.</summary>
public sealed record GetOrderProcessingQueueQuery : IRequest<Result<OrderQueueResponse>>;

using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Processing.GetExpressDispatchQueue;

/// <summary>Query: ordini in attesa nell'ordine di dispatch per priorità (express).</summary>
public sealed record GetExpressDispatchQueueQuery : IRequest<Result<OrderQueueResponse>>;

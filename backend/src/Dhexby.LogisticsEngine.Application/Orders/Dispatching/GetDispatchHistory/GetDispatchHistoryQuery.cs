using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.GetDispatchHistory;

/// <summary>Query: stato corrente della cronologia di dispatch (undo/redo).</summary>
public sealed record GetDispatchHistoryQuery : IRequest<Result<DispatchHistoryResponse>>;

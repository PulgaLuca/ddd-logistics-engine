using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching.GetDispatchHistory;

/// <summary>Use case di lettura: proietta lo stato della cronologia di dispatch.</summary>
public sealed class GetDispatchHistoryQueryHandler
    : IRequestHandler<GetDispatchHistoryQuery, Result<DispatchHistoryResponse>>
{
    private readonly IDispatchHistory _dispatchHistory;

    public GetDispatchHistoryQueryHandler(IDispatchHistory dispatchHistory)
    {
        _dispatchHistory = dispatchHistory;
    }

    public Task<Result<DispatchHistoryResponse>> Handle(
        GetDispatchHistoryQuery request,
        CancellationToken cancellationToken)
    {
        Result<DispatchHistoryResponse> result = DispatchHistoryResponse.From(_dispatchHistory);

        return Task.FromResult(result);
    }
}

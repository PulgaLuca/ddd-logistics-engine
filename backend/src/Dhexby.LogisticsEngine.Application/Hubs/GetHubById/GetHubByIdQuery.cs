using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Hubs.GetHubById;

public sealed record GetHubByIdQuery(Guid HubId) : IRequest<Result<HubResponse>>;

namespace LogisticsEngine.Application.Hubs;

public sealed record HubConnectionDto(Guid FirstHubId, Guid SecondHubId, decimal Distance);

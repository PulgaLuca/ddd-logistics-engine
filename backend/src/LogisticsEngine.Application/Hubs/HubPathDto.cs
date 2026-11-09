namespace LogisticsEngine.Application.Hubs;

public sealed record HubPathDto(IReadOnlyList<HubDto> Hubs, decimal TotalDistance);

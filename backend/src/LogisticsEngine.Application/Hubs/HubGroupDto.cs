namespace LogisticsEngine.Application.Hubs;

public sealed record HubGroupDto(string Zone, IReadOnlyList<HubDto> Hubs);

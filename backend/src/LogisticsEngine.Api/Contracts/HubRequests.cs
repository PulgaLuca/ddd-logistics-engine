using LogisticsEngine.Domain.Shipments.Enums;

namespace LogisticsEngine.Api.Contracts;

/// <summary>Payload HTTP per creare un Hub.</summary>
public sealed record CreateHubRequest(string? Name, string? Zone);

/// <summary>Payload HTTP per modificare un Hub.</summary>
public sealed record UpdateHubRequest(string? Name, string? Zone);

/// <summary>Payload HTTP per aggiungere un arco bidirezionale al grafo degli Hub.</summary>
public sealed record CreateHubConnectionRequest(Guid FirstHubId, Guid SecondHubId, decimal Distance);

/// <summary>Payload HTTP per richiedere una transizione di stato della spedizione.</summary>
public sealed record UpdateShipmentStatusRequest(ShipmentStatus Status);

/// <summary>Payload HTTP per creare una spedizione.</summary>
public sealed record CreateShipmentRequest(string? TrackingNumber);

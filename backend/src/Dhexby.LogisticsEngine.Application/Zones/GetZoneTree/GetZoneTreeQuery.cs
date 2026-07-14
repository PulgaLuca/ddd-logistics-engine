using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneTree;

/// <summary>Query: albero completo (zona radice -&gt; sottozone -&gt; vie).</summary>
public sealed record GetZoneTreeQuery(Guid RootZoneId) : IRequest<Result<ZoneTreeResponse>>;

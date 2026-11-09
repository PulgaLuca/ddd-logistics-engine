using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Hubs.Vo;

/// <summary>Arco bidirezionale pesato (distanza) tra due Hub.</summary>
public sealed class HubConnection : ValueObject
{
    private HubConnection(Guid firstHubId, Guid secondHubId, decimal distance)
    {
        if (firstHubId.CompareTo(secondHubId) <= 0)
        {
            FirstHubId = firstHubId;
            SecondHubId = secondHubId;
        }
        else
        {
            FirstHubId = secondHubId;
            SecondHubId = firstHubId;
        }

        Distance = distance;
    }

    public Guid FirstHubId { get; }
    public Guid SecondHubId { get; }
    public decimal Distance { get; }

    public static Result<HubConnection> Create(Guid firstHubId, Guid secondHubId, decimal distance)
    {
        if (firstHubId == Guid.Empty || secondHubId == Guid.Empty || firstHubId == secondHubId || distance <= 0)
        {
            return Result.Failure<HubConnection>(HubErrors.InvalidConnection);
        }

        return Result.Success(new HubConnection(firstHubId, secondHubId, distance));
    }

    protected override IEnumerable<object> GetAtomicValues() => [FirstHubId, SecondHubId, Distance];
}

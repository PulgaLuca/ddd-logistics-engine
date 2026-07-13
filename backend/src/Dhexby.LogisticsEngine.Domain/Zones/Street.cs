using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Zones;

/// <summary>
/// Value Object: via appartenente a una zona (foglia dell'albero delle zone).
/// L'uguaglianza strutturale consente di evitare duplicati nell'aggregato.
/// </summary>
public sealed class Street : ValueObject
{
    private Street(string name, string postalCode)
    {
        Name = name;
        PostalCode = postalCode;
    }

    public string Name { get; }

    public string PostalCode { get; }

    public static Result<Street> Create(string? name, string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(postalCode))
        {
            return Result.Failure<Street>(ZoneErrors.StreetIncomplete);
        }

        return new Street(name.Trim(), postalCode.Trim());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return PostalCode;
    }
}

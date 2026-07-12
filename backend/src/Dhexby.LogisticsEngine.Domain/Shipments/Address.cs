using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Value Object: indirizzo di origine o destinazione di una spedizione.
/// Tutti i campi sono obbligatori; l'uguaglianza è strutturale.
/// </summary>
public sealed class Address : ValueObject
{
    private Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public string Street { get; }

    public string City { get; }

    public string PostalCode { get; }

    public string Country { get; }

    public static Result<Address> Create(string? street, string? city, string? postalCode, string? country)
    {
        if (string.IsNullOrWhiteSpace(street) ||
            string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(postalCode) ||
            string.IsNullOrWhiteSpace(country))
        {
            return Result.Failure<Address>(ShipmentErrors.AddressIncomplete);
        }

        return new Address(street.Trim(), city.Trim(), postalCode.Trim(), country.Trim());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}

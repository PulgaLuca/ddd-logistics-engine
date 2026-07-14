using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Hubs;

/// <summary>
/// Value Object: coordinata geografica (WGS84). Espone la distanza in km
/// (formula dell'emisenoverso), utile come peso degli archi e come euristica
/// per i futuri algoritmi di routing (Dijkstra / A*).
/// </summary>
public sealed class GeoCoordinate : ValueObject
{
    private const double _earthRadiusKilometers = 6371.0;

    private GeoCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; }

    public double Longitude { get; }

    public static Result<GeoCoordinate> Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            return Result.Failure<GeoCoordinate>(HubErrors.LatitudeOutOfRange);
        }

        if (longitude is < -180 or > 180)
        {
            return Result.Failure<GeoCoordinate>(HubErrors.LongitudeOutOfRange);
        }

        return new GeoCoordinate(latitude, longitude);
    }

    /// <summary>Distanza in chilometri da un'altra coordinata (great-circle).</summary>
    public double DistanceInKilometersTo(GeoCoordinate other)
    {
        ArgumentNullException.ThrowIfNull(other);

        double dLat = ToRadians(other.Latitude - Latitude);
        double dLon = ToRadians(other.Longitude - Longitude);

        double a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) +
                   (Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2));

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return _earthRadiusKilometers * c;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Latitude;
        yield return Longitude;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}

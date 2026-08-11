using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Converter riutilizzabili dal mapping EF Core.
/// </summary>
internal static class PersistenceConverters
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Npgsql accetta solo <see cref="DateTime"/> con Kind=Utc sulle colonne
    /// <c>timestamptz</c>. Il dominio usa già DateTime.UtcNow, ma normalizzare
    /// in entrambe le direzioni evita eccezioni con dati caricati da altri store.
    /// </summary>
    public static readonly ValueConverter<DateTime, DateTime> UtcDateTime = new(
        value => value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc),
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

    /// <summary>Variante nullable di <see cref="UtcDateTime"/>.</summary>
    public static readonly ValueConverter<DateTime?, DateTime?> NullableUtcDateTime = new(
        value => value == null
            ? null
            : value.Value.Kind == DateTimeKind.Utc
                ? value
                : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc),
        value => value == null ? null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc));

    /// <summary>
    /// L'indirizzo è un Value Object: non ha identità e si sostituisce in blocco.
    /// Viene quindi salvato come singola colonna <c>jsonb</c> anziché esploso in
    /// quattro colonne, così resta atomico e il record di persistenza continua a
    /// essere costruibile dal costruttore posizionale (EF Core lega al costruttore
    /// solo proprietà scalari, non navigazioni verso tipi owned).
    /// </summary>
    public static readonly ValueConverter<AddressDocument, string> Address = new(
        address => JsonSerializer.Serialize(address, _jsonOptions),
        json => JsonSerializer.Deserialize<AddressDocument>(json, _jsonOptions)!);

    /// <summary>
    /// Le vie di una zona sono parte dell'aggregato Zone e non hanno vita propria:
    /// vengono serializzate in un'unica colonna <c>jsonb</c> invece di diventare
    /// una tabella figlia. Così il confine dell'aggregato resta uno solo anche
    /// nello schema relazionale.
    /// </summary>
    public static readonly ValueConverter<IReadOnlyList<StreetDocument>, string> Streets = new(
        streets => JsonSerializer.Serialize(streets, _jsonOptions),
        json => JsonSerializer.Deserialize<List<StreetDocument>>(json, _jsonOptions) ?? new List<StreetDocument>());

    /// <summary>
    /// Comparer esplicito: senza, EF Core non saprebbe rilevare le modifiche
    /// a una collection mappata come valore singolo.
    /// </summary>
    public static readonly ValueComparer<IReadOnlyList<StreetDocument>> StreetsComparer = new(
        (left, right) => left != null && right != null && left.SequenceEqual(right),
        streets => streets.Aggregate(0, (hash, street) => HashCode.Combine(hash, street.GetHashCode())),
        streets => streets.ToList());
}

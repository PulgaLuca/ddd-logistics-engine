using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Mapping relazionale della tabella <c>logistics.roads</c> (archi del grafo).</summary>
internal sealed class RoadConfiguration : IEntityTypeConfiguration<RoadDocument>
{
    public void Configure(EntityTypeBuilder<RoadDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("roads");

        builder.HasKey(road => road.Id);
        builder.Property(road => road.Id).HasColumnName("id");

        builder.Property(road => road.FromHubId).HasColumnName("from_hub_id");
        builder.Property(road => road.ToHubId).HasColumnName("to_hub_id");

        builder.Property(road => road.DistanceInKilometers).HasColumnName("distance_in_kilometers");
        builder.Property(road => road.TrafficFactor).HasColumnName("traffic_factor");

        builder.Property(road => road.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .HasConversion(PersistenceConverters.UtcDateTime);

        // Indice a supporto di ExistsBetweenAsync e della ricostruzione del grafo.
        builder.HasIndex(road => new { road.FromHubId, road.ToHubId });
    }
}

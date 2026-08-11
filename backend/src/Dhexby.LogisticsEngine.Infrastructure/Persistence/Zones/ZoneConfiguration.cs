using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Mapping relazionale della tabella <c>logistics.zones</c>.</summary>
internal sealed class ZoneConfiguration : IEntityTypeConfiguration<ZoneDocument>
{
    public void Configure(EntityTypeBuilder<ZoneDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("zones");

        builder.HasKey(zone => zone.Id);
        builder.Property(zone => zone.Id).HasColumnName("id");

        builder.Property(zone => zone.Code)
            .HasColumnName("code")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(zone => zone.Code).IsUnique();

        builder.Property(zone => zone.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        // L'albero delle zone è un riferimento per id verso un altro aggregato:
        // nessuna self-referencing foreign key, la gerarchia si naviga a livello
        // applicativo (GetChildrenAsync).
        builder.Property(zone => zone.ParentZoneId)
            .HasColumnName("parent_zone_id");

        builder.HasIndex(zone => zone.ParentZoneId);

        builder.Property(zone => zone.Streets)
            .HasColumnName("streets")
            .HasColumnType("jsonb")
            .HasConversion(PersistenceConverters.Streets, PersistenceConverters.StreetsComparer)
            .IsRequired();

        builder.Property(zone => zone.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .HasConversion(PersistenceConverters.UtcDateTime);
    }
}

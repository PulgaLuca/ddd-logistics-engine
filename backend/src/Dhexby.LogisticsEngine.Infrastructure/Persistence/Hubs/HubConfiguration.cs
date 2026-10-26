using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Mapping relazionale della tabella <c>logistics.hubs</c>.</summary>
internal sealed class HubConfiguration : IEntityTypeConfiguration<HubDocument>
{
    public void Configure(EntityTypeBuilder<HubDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("hubs");

        builder.HasKey(hub => hub.Id);
        builder.Property(hub => hub.Id).HasColumnName("id");

        builder.Property(hub => hub.Code)
            .HasColumnName("code")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(hub => hub.Code).IsUnique();

        //builder.Property(hub => hub.Name)
        //    .HasColumnName("name")
        //    .HasMaxLength(150)
        //    .IsRequired();

        builder.Property(hub => hub.Latitude).HasColumnName("latitude");
        builder.Property(hub => hub.Longitude).HasColumnName("longitude");

        builder.Property(hub => hub.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(hub => hub.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .HasConversion(PersistenceConverters.UtcDateTime);
    }
}

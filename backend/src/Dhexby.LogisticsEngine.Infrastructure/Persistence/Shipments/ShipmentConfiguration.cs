using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Mapping relazionale della tabella <c>logistics.shipments</c>.</summary>
internal sealed class ShipmentConfiguration : IEntityTypeConfiguration<ShipmentDocument>
{
    public void Configure(EntityTypeBuilder<ShipmentDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("shipments");

        builder.HasKey(shipment => shipment.Id);
        builder.Property(shipment => shipment.Id).HasColumnName("id");

        builder.Property(shipment => shipment.TrackingNumber)
            .HasColumnName("tracking_number")
            .HasMaxLength(20)
            .IsRequired();

        // Invariante di dominio resa esplicita anche a livello di schema:
        // il tracking number identifica univocamente la spedizione.
        builder.HasIndex(shipment => shipment.TrackingNumber).IsUnique();

        builder.Property(shipment => shipment.Origin)
            .HasColumnName("origin")
            .HasColumnType("jsonb")
            .HasConversion(PersistenceConverters.Address)
            .IsRequired();

        builder.Property(shipment => shipment.Destination)
            .HasColumnName("destination")
            .HasColumnType("jsonb")
            .HasConversion(PersistenceConverters.Address)
            .IsRequired();

        builder.Property(shipment => shipment.WeightInKilograms)
            .HasColumnName("weight_in_kilograms")
            .HasPrecision(9, 3);

        builder.Property(shipment => shipment.PriceAmount)
            .HasColumnName("price_amount")
            .HasPrecision(12, 2);

        builder.Property(shipment => shipment.PriceCurrency)
            .HasColumnName("price_currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(shipment => shipment.IsExpress)
            .HasColumnName("is_express");

        // Enum salvato come testo: il dump del database resta leggibile e
        // l'aggiunta di un nuovo stato non rinumera quelli esistenti.
        builder.Property(shipment => shipment.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(shipment => shipment.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .HasConversion(PersistenceConverters.UtcDateTime);

        builder.Property(shipment => shipment.DeliveredOnUtc)
            .HasColumnName("delivered_on_utc")
            .HasConversion(PersistenceConverters.NullableUtcDateTime);

        builder.HasIndex(shipment => shipment.Status);
    }
}

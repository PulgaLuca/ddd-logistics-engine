using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Mapping relazionale della tabella <c>logistics.orders</c>.</summary>
internal sealed class OrderConfiguration : IEntityTypeConfiguration<OrderDocument>
{
    public void Configure(EntityTypeBuilder<OrderDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("orders");

        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id).HasColumnName("id");

        builder.Property(order => order.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(order => order.OrderNumber).IsUnique();

        builder.Property(order => order.CustomerName)
            .HasColumnName("customer_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(order => order.Priority)
            .HasColumnName("priority")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // Riferimento per id verso un altro aggregate root: nessuna foreign key,
        // perché ogni aggregato è un confine transazionale a sé stante.
        builder.Property(order => order.AssignedHubId)
            .HasColumnName("assigned_hub_id");

        builder.Property(order => order.PlacedOnUtc)
            .HasColumnName("placed_on_utc")
            .HasConversion(PersistenceConverters.UtcDateTime);

        // Indice a supporto di GetPendingAsync (coda di processamento ordini).
        builder.HasIndex(order => order.Status);
    }
}

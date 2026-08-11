using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Contesto EF Core su PostgreSQL. Mappa i <c>*Document</c> (modelli di
/// persistenza fatti di soli primitivi) e non gli aggregati: il dominio resta
/// privo di attributi, colonne e chiavi esterne, esattamente come per i
/// provider Json e Mongo.
/// <para>
/// Il change tracker di questo contesto è il "registro" dell'Unit of Work:
/// i repository ci scrivono dentro, ma le modifiche diventano definitive solo
/// quando <see cref="EfUnitOfWork"/> chiama SaveChanges dentro la transazione
/// aperta dalla pipeline.
/// </para>
/// </summary>
public sealed class LogisticsDbContext : DbContext
{
    /// <summary>Schema dedicato: tiene le tabelle applicative separate da "public".</summary>
    public const string Schema = "logistics";

    public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options)
        : base(options)
    {
    }

    internal DbSet<ShipmentDocument> Shipments => Set<ShipmentDocument>();

    internal DbSet<OrderDocument> Orders => Set<OrderDocument>();

    internal DbSet<HubDocument> Hubs => Set<HubDocument>();

    internal DbSet<ZoneDocument> Zones => Set<ZoneDocument>();

    internal DbSet<RoadDocument> Roads => Set<RoadDocument>();

    internal DbSet<UserDocument> Users => Set<UserDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

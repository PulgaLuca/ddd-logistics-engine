using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Mapping relazionale della tabella <c>logistics.users</c>.</summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<UserDocument>
{
    public void Configure(EntityTypeBuilder<UserDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).HasColumnName("id");

        builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();

        // Solo l'hash PBKDF2, mai la password in chiaro.
        builder.Property(user => user.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(user => user.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(user => user.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .HasConversion(PersistenceConverters.UtcDateTime);
    }
}

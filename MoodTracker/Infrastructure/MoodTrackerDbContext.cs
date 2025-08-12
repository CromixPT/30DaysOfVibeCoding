using Microsoft.EntityFrameworkCore;

namespace MoodTracker.Infrastructure;

/// <summary>
/// EF Core DbContext for mood tracking persistence.
/// </summary>
public sealed class MoodTrackerDbContext : DbContext
{
    public MoodTrackerDbContext(DbContextOptions<MoodTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<MoodEntryEntity> MoodEntries => Set<MoodEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<MoodEntryEntity>();
        e.HasKey(x => x.Id);
        e.Property(x => x.MoodKey).HasMaxLength(64).IsRequired();
        e.Property(x => x.MoodName).HasMaxLength(128).IsRequired();
        e.Property(x => x.Emoji).HasMaxLength(8).IsRequired();
        e.Property(x => x.Sentiment).HasMaxLength(32).IsRequired();
        e.Property(x => x.Emotion).HasMaxLength(64);
        e.Property(x => x.Message).HasMaxLength(1024);
        e.HasIndex(x => x.TimestampUtc);
        e.HasQueryFilter(x => x.DeletedAt == null);

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<MoodEntryEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

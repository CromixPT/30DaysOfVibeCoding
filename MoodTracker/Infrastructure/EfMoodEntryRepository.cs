using Microsoft.EntityFrameworkCore;
using MoodTracker.Services; // For IMoodEntryRepository + MoodEntry record

namespace MoodTracker.Infrastructure;

/// <summary>
/// EF Core implementation of mood entry repository using SQLite persistence.
/// </summary>
public sealed class EfMoodEntryRepository : IMoodEntryRepository
{
    private readonly MoodTrackerDbContext db;

    public EfMoodEntryRepository(MoodTrackerDbContext db)
    {
        this.db = db;
    }

    public async Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default)
    {
        var entity = new MoodEntryEntity
        {
            Id = entry.Id,
            MoodKey = entry.MoodKey,
            MoodName = entry.MoodName,
            Emoji = entry.Emoji,
            Sentiment = entry.Sentiment,
            Emotion = entry.Emotion,
            Message = entry.Message,
            TimestampUtc = entry.TimestampUtc
        };
    await db.MoodEntries.AddAsync(entity, cancellationToken);
    await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoodEntry>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default)
    {
    var list = await db.MoodEntries
            .OrderByDescending(e => e.TimestampUtc)
            .Take(count)
            .AsNoTracking()
            .Select(e => new MoodEntry(e.Id, e.MoodKey, e.MoodName, e.Emoji, e.Sentiment, e.Emotion, e.Message, e.TimestampUtc))
            .ToListAsync(cancellationToken);
        return list;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => db.MoodEntries.CountAsync(cancellationToken);

    public async Task<IReadOnlyList<MoodEntry>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = start.AddDays(1);
        var list = await db.MoodEntries
            .Where(e => e.TimestampUtc >= start && e.TimestampUtc < end)
            .OrderByDescending(e => e.TimestampUtc)
            .AsNoTracking()
            .Select(e => new MoodEntry(e.Id, e.MoodKey, e.MoodName, e.Emoji, e.Sentiment, e.Emotion, e.Message, e.TimestampUtc))
            .ToListAsync(cancellationToken);
        return list;
    }

    /// <summary>
    /// Bulk range retrieval to support week/month calendar views without issuing one query per day (avoids N+1 patterns).
    /// </summary>
    public async Task<IReadOnlyList<MoodEntry>> GetRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        var list = await db.MoodEntries
            .Where(e => e.TimestampUtc >= fromUtc && e.TimestampUtc < toUtc)
            .OrderByDescending(e => e.TimestampUtc) // newest first; caller may regroup
            .AsNoTracking()
            .Select(e => new MoodEntry(e.Id, e.MoodKey, e.MoodName, e.Emoji, e.Sentiment, e.Emotion, e.Message, e.TimestampUtc))
            .ToListAsync(cancellationToken);
        return list;
    }
}

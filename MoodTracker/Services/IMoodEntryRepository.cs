namespace MoodTracker.Services;

/// <summary>
/// Abstraction for persisting and querying mood/diary entries.
/// In-memory implementation is thread-safe and keeps data for app lifetime.
/// </summary>
public interface IMoodEntryRepository
{
    /// <summary>Add a new entry.</summary>
    /// <param name="entry">Entry to store.</param>
    Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Return most recent entries (newest first).</summary>
    /// <param name="count">Maximum number of entries.</param>
    Task<IReadOnlyList<MoodEntry>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>Total number of stored entries.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Return entries for the specified local date (newest first).</summary>
    Task<IReadOnlyList<MoodEntry>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return all entries whose UTC timestamp falls within the inclusive <paramref name="fromUtc"/> (>=) and exclusive <paramref name="toUtc"/> (<) range.
    /// Caller is responsible for any grouping/ordering beyond the returned default ordering (newest first).
    /// </summary>
    /// <param name="fromUtc">Inclusive range start (UTC).</param>
    /// <param name="toUtc">Exclusive range end (UTC).</param>
    /// <remarks>
    /// Added to eliminate N+1 query patterns when loading week/month calendar views. Optimizes database round-trips by fetching a contiguous block in one query.
    /// </remarks>
    Task<IReadOnlyList<MoodEntry>> GetRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
}

/// <summary>
/// Immutable record representing a diary entry persisted in the repository layer.
/// </summary>
/// <param name="Id">Unique identifier.</param>
/// <param name="MoodKey">Stable mood key (e.g. "happy").</param>
/// <param name="MoodName">Display name of the mood.</param>
/// <param name="Emoji">Emoji associated with the mood.</param>
/// <param name="Sentiment">Sentiment bucket used for contextual messaging.</param>
/// <param name="Emotion">Optional secondary emotion tag.</param>
/// <param name="Message">Support message stored with the mood.</param>
/// <param name="TimestampUtc">UTC timestamp when created.</param>
public sealed record MoodEntry(Guid Id, string MoodKey, string MoodName, string Emoji, string Sentiment, string? Emotion, string? Message, DateTime TimestampUtc);

using System.Collections.Concurrent;

namespace MoodTracker.Services;

/// <summary>
/// Thread-safe in-memory repository for mood entries. Suitable for demo / dev scenarios only.
/// </summary>
public sealed class InMemoryMoodEntryRepository : IMoodEntryRepository
{
    // ConcurrentQueue keeps insertion order; newest logic handled on query.
    private readonly ConcurrentQueue<MoodEntry> entries = new();
    private readonly int maxEntries;

    public InMemoryMoodEntryRepository(int maxEntries = 2000)
    {
        this.maxEntries = maxEntries;
    }

    public Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default)
    {
        entries.Enqueue(entry);
        // Trim if exceeding capacity (non-critical best-effort loop)
        while (entries.Count > maxEntries && entries.TryDequeue(out _)) { }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<MoodEntry>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        // Snapshot to array then take last N reversed (newest first)
        var snapshot = entries.ToArray();
        var result = snapshot
            .Skip(Math.Max(0, snapshot.Length - count))
            .Reverse()
            .ToArray();
        return Task.FromResult<IReadOnlyList<MoodEntry>>(result);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(entries.Count);

    public Task<IReadOnlyList<MoodEntry>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var snapshot = entries.ToArray();
        var target = snapshot
            .Where(e => DateOnly.FromDateTime(e.TimestampUtc) == date)
            .OrderByDescending(e => e.TimestampUtc)
            .ToArray();
        return Task.FromResult<IReadOnlyList<MoodEntry>>(target);
    }
}

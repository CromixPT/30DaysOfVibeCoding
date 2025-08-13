using Microsoft.EntityFrameworkCore;

namespace MoodTracker.Infrastructure;

/// <summary>
/// Data access object (EF Core entity) representing a stored mood entry.
/// Includes auditing timestamps for create/update/delete soft-deletion semantics.
/// </summary>
[Index(nameof(TimestampUtc))]
public sealed class MoodEntryEntity
{
    public Guid Id { get; set; }
    public string MoodKey { get; set; } = string.Empty;
    public string MoodName { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string? Emotion { get; set; }
    public string? Message { get; set; }
    public DateTime TimestampUtc { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

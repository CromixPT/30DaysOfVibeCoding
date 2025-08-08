using Microsoft.Extensions.Hosting;

namespace MoodTracker.Services;

/// <summary>
/// On application start seeds one random mood entry for every past day of the current month
/// if the repository is currently empty. This gives the diary some historical data.
/// </summary>
public sealed class MonthlySeedHostedService : IHostedService
{
    private readonly IMoodEntryRepository repository;
    private readonly ILogger<MonthlySeedHostedService> logger;

    // Static mood definitions mirrored from Diary component (keep in sync or externalize later).
    private static readonly (string Key,string Name,string Emoji,string Sentiment)[] moods = new []
    {
        ("happy","Happy","😄","Positive"),
        ("sad","Sad","😢","Negative"),
        ("excited","Excited","🤩","Positive"),
        ("tired","Tired","😴","Negative"),
        ("calm","Calm","😌","Positive")
    };

    private static readonly string[] positiveMessages =
    [
        "Love that energy—keep it going!",
        "You're radiating great vibes today.",
        "Channel that mood into something awesome!",
        "Keep the momentum—you're doing great.",
        "That smile is your superpower today."
    ];
    private static readonly string[] negativeMessages =
    [
        "It's okay to feel this way—small steps count.",
        "Be gentle with yourself; you matter.",
        "Even tough moments pass—you're not alone.",
        "Rest is productive—take what you need.",
        "Your feelings are valid; brighter moments are coming."
    ];
    private static readonly string[] neutralMessages =
    [
        "Steady is a fine place to be.",
        "A calm moment can recharge everything.",
        "Use this space to reflect or plan something small.",
        "Neutral today leaves room for surprise later.",
        "A balanced mood sets a solid foundation."
    ];

    public MonthlySeedHostedService(IMoodEntryRepository repository, ILogger<MonthlySeedHostedService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await repository.CountAsync(cancellationToken) > 0)
            {
                return; // Already seeded / has data.
            }
            var today = DateTime.UtcNow.Date;
            var first = new DateTime(today.Year, today.Month, 1, 12,0,0,DateTimeKind.Utc); // midday for consistency
            var daysToSeed = (today.Day - 1); // past days only
            var rng = Random.Shared;
            for (int i = 0; i < daysToSeed; i++)
            {
                var date = first.AddDays(i);
                var mood = moods[rng.Next(moods.Length)];
                string[] pool = mood.Sentiment switch
                {
                    "Positive" => positiveMessages,
                    "Negative" => negativeMessages,
                    _ => neutralMessages
                };
                var message = pool.Length == 0 ? null : pool[rng.Next(pool.Length)];
                var entry = new MoodEntry(Guid.NewGuid(), mood.Key, mood.Name, mood.Emoji, mood.Sentiment, null, message, date);
                await repository.AddAsync(entry, cancellationToken);
            }
            logger.LogInformation("Seeded {Count} mood entries for current month.", daysToSeed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding monthly mood data");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

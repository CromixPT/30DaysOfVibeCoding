using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoodTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MoodEntries",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Emoji", "Emotion", "Message", "MoodKey", "MoodName", "Sentiment", "TimestampUtc", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "😄", null, "Seed: Kicking off the year with good vibes!", "happy", "Happy", "Positive", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc), null, "😌", null, "Seed: Steady and balanced.", "calm", "Calm", "Positive", new DateTime(2025, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MoodEntries",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MoodEntries",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}

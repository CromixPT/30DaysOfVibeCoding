using Microsoft.EntityFrameworkCore;
using MoodTracker.Components;
using MoodTracker.Services; // IMoodEntryRepository
using MoodTracker.Infrastructure; // EF Core persistence layer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure EF Core with SQLite (scoped DbContext per request) – standard pattern.
var connectionString = builder.Configuration.GetConnectionString("Diary") ?? "Data Source=diary.db"; // fallback for safety
builder.Services.AddDbContext<MoodTrackerDbContext>(options => options.UseSqlite(connectionString));

// Repository abstraction (scoped per request since DbContext is scoped).
builder.Services.AddScoped<IMoodEntryRepository, EfMoodEntryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

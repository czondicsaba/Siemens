using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Repositories;
using Siemens.Internship2026.GradeBook.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IItemReader, ExternalItemRepository>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "GradeBook-API/1.0");
});

builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<IStatisticsService, DetailedStatisticsService>();
builder.Services.AddScoped<IGradeFilterService, GradeFilterService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggingService>();
    var reader = scope.ServiceProvider.GetRequiredService<IItemReader>();

    try
    {
        logger.Log("Pre-loading external data at application startup...");
        await reader.RefreshDataAsync();
        logger.Log($"Data loaded successfully. Last updated: {reader.LastUpdated}");
    }
    catch (Exception ex)
    {
        logger.Log($"Warning: Could not pre-load external data: {ex.Message}");
        logger.Log("Application will continue, but first request may be slower");
    }
}
app.Run();
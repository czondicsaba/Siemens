using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;
public class GradeFilterService : IGradeFilterService
{
    private readonly IItemReader _itemReader;
    private readonly ILoggingService _logger;
    private const int PASSING_GRADE_THRESHOLD = 5;
    public GradeFilterService(IItemReader itemReader, ILoggingService logger)
    {
        _itemReader = itemReader;
        _logger = logger;
    }
    public async Task<IEnumerable<Item>> GetFirstNPassingGradesAsync(IEnumerable<Item> items, int count)
    {
        if (count <= 0)
        {
            _logger.Log($"Invalid count parameter: {count}. Must be positive.");
            return Enumerable.Empty<Item>();
        }

        var result = items
            .Where(i => i.IsActive && i.Value >= PASSING_GRADE_THRESHOLD)
            .Take(count)
            .ToList();
        _logger.Log($"Filtered {result.Count} items out of {items.Count()} total. Requested N={count}");

        return result;
    }
    public async Task<IEnumerable<Item>> GetFirstNPassingGradesFromRepositoryAsync(int count)
    {
        _logger.Log($"Fetching first {count} passing and active grades from repository");
        var allItems = await _itemReader.GetAllAsync();
        return await GetFirstNPassingGradesAsync(allItems, count);
    }
}

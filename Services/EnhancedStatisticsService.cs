using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public class EnhancedStatisticsService : IStatisticsService
{
    private readonly IGradeFilterService _gradeFilterService;
    private readonly IStatisticsService _innerStatisticsService;
    public EnhancedStatisticsService(
        IGradeFilterService gradeFilterService,
        IStatisticsService innerStatisticsService)
    {
        _gradeFilterService = gradeFilterService;
        _innerStatisticsService = innerStatisticsService;
    }
    public ItemStatistics CalculateStatistics(IEnumerable<Item> items)
    {
        return _innerStatisticsService.CalculateStatistics(items);
    }
    public async Task<ItemStatistics> CalculateStatisticsForFirstNPassingGradesAsync(
        IEnumerable<Item> items,
        int count)
    {
        var filteredItems = await _gradeFilterService.GetFirstNPassingGradesAsync(items, count);
        return _innerStatisticsService.CalculateStatistics(filteredItems);
    }
}

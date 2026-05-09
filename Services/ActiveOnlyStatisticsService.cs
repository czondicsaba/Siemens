using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services
{
    public class ActiveOnlyStatisticsService: IStatisticsService
    {
        private readonly IStatisticsService _innerService;
        public ActiveOnlyStatisticsService(IStatisticsService innerService)
        {
            _innerService = innerService;
        }
        public ItemStatistics CalculateStatistics(IEnumerable<Item> items)
        {
            var activeItems = items.Where(i => i.IsActive);
            return _innerService.CalculateStatistics(activeItems);
        }
    }
}

using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services
{
    public class BasicStatisticsService : IStatisticsService
    {
        public ItemStatistics CalculateStatistics(IEnumerable<Item> items)
        {
            var itemList = items.ToList();
            return new ItemStatistics
            {
                TotalCount = itemList.Count,
                AverageValue = itemList.Any() ? (double)itemList.Average(i => i.Value) : 0,
                RetrievedAt = DateTime.UtcNow
            };
        }
    }
}

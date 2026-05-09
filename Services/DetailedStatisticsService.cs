using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services
{
    public class DetailedStatisticsService: IStatisticsService
    {
        public ItemStatistics CalculateStatistics(IEnumerable<Item> items)
        {
            var itemList = items.ToList();
            if (!itemList.Any())
            {
                return new ItemStatistics
                {
                    TotalCount = 0,
                    AverageValue = 0,
                    RetrievedAt = DateTime.UtcNow
                };
            }
            var values = itemList.Select(i => (double)i.Value);
            return new ItemStatistics
            {
                TotalCount = itemList.Count,
                AverageValue = values.Average(),
                MinValue = values.Min(),        
                MaxValue = values.Max(),        
                Sum = values.Sum(),             
                RetrievedAt = DateTime.UtcNow
            };
        }
    }
}

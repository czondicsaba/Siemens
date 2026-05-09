using Siemens.Internship2026.GradeBook.Models;
using Siemens.Internship2026.GradeBook.Services;

namespace Siemens.Internship2026.GradeBook.Interfaces
{
    public interface IStatisticsService
    {
        ItemStatistics CalculateStatistics(IEnumerable<Item> items);
    }
}

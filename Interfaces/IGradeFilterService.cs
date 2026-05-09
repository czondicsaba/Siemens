using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces
{
    public interface IGradeFilterService
    {
        Task<IEnumerable<Item>> GetFirstNPassingGradesAsync(IEnumerable<Item> items, int count);
        Task<IEnumerable<Item>> GetFirstNPassingGradesFromRepositoryAsync(int count);
    }
}

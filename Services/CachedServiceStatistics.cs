using Microsoft.Extensions.Caching.Memory;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services
{
    public class CachedServiceStatistics: IStatisticsService
    {
        private readonly IStatisticsService _innerService;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
        public CachedServiceStatistics(IStatisticsService innerService, IMemoryCache cache)
        {
            _innerService = innerService;
            _cache = cache;
        }
        public ItemStatistics CalculateStatistics(IEnumerable<Item> items)
        {
            var cacheKey = $"stats_{string.Join(",", items.Select(i => $"{i.Id}_{i.Value}_{i.IsActive}"))}";

            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return _innerService.CalculateStatistics(items);
            });
        }
    }
}

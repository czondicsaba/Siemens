namespace Siemens.Internship2026.GradeBook.Models
{
    public class ItemStatistics
    {
        public int TotalCount { get; set; }
        public double AverageValue { get; set; }
        public DateTime RetrievedAt { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? Sum { get; set; }
        public double? Median { get; set; }
        public double? StandardDeviation { get; set; }
        public ItemStatistics()
        {
            RetrievedAt = DateTime.UtcNow;
        }
    }
}

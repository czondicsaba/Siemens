using System.Text.Json.Serialization;

namespace Siemens.Internship2026.GradeBook.Dtos
{
    public class ExternalItemDtos
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
    public class ExternalItemsResponse
    {
        [JsonPropertyName("items")]
        public List<ExternalItemDtos> Items { get; set; } = new();
    }
}

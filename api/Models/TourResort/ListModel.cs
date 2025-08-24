using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Waffle.Core.Foundations;
using Waffle.Entities.TourResort;

namespace Waffle.Models.TourResort
{
    public class ListModel : BaseSetting
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
        [JsonPropertyName("rating")]
        public double Rating { get; set; } = default!;
        [JsonPropertyName("point")]
        public int Point { get; set; } = default!;
        [JsonPropertyName("summary")]
        public string? Summary { get; set; } = default!;
        [JsonPropertyName("include")]
        public string? Include { get; set; } = default!;
        [JsonPropertyName("exclude")]
        public string? Exclude { get; set; } = default!;
        [JsonPropertyName("address")]
        public string? Address { get; set; } = default!;
        [JsonPropertyName("duration")]
        public string? Duration { get; set; } = default!;
        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; } = default!;
    }
}

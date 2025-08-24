using System.Text.Json.Serialization;
using Waffle.Core.Foundations;
using Waffle.Entities.TourResort;

namespace Waffle.Models.TourResort;

public class DetailModel : BaseSetting
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    [JsonPropertyName("rating")]
    public double Rating { get; set; } = default!;
    [JsonPropertyName("imageUrls")]
    public List<string> ImageUrls { get; set; } = default!;
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
    [JsonPropertyName("include")]
    public string? Include { get; set; }
    [JsonPropertyName("exclude")]
    public string? Exclude { get; set; }
    [JsonPropertyName("itineraries")]
    public List<TourResortItinerary> Itineraries { get; set; } = default!;
    [JsonPropertyName("comments")]
    public List<TourResortComment> Comments { get; set; } = default!;
}

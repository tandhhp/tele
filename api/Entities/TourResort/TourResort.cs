using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities.TourResort
{
    public class TourResort : TourResortBase
    {
        [MaxLength(2000)]
        public string Name { get; set; } = default!;
        public int Point { get; set; }
        public double Rating { get; set; } = 5;
        [MaxLength(4000)]
        public string? Summary { get; set; }
        [MaxLength(4000)]
        public string? Include { get; set; }
        [MaxLength(4000)]
        public string? Exclude { get; set; }
        [MaxLength(2000)]
        public string? Address { get; set; }
        [MaxLength(2000)]
        public string? Duration { get; set; }
    }
}

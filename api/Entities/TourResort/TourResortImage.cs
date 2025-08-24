using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities.TourResort
{
    public class TourResortImage : TourResortBase
    {
        public Guid TourResortId { get; set; }
        public string Url { get; set; } = default!;
        [MaxLength(255)]
        public string? Alt { get; set; }
    }
}

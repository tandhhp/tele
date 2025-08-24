namespace Waffle.Entities.TourResort
{
    public class TourResortItinerary : TourResortBase
    {
        public Guid TourResortId { get; set; }
        public string ImageUrl { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string RichText { get; set; } = default!;
    }
}

namespace Waffle.Entities.TourResort
{
    public class TourResortComment : TourResortBase
    {
        public Guid TourResortId { get; set; }
        public string Name { get; set; } = default!;
        public string Comment { get; set; } = default!;
        public double Rating { get; set; } = 5;
    }
}

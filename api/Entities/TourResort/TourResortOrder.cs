namespace Waffle.Entities.TourResort
{
    public class TourResortOrder : TourResortBase
    {
        public Guid TourResortId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public int? NumberOfAdult { get; set; }
        public int? NumberOfChildren { get; set; }
    }
}

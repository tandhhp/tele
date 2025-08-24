namespace Waffle.Entities.TourResort
{
    public class TourResortBase : BaseEntity
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}

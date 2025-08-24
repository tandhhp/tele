using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities.Plasma;

public class PlasmaCheckIn : BaseEntity
{
    public Guid PlasmaUserId { get; set; }
    public DateTime Date {  get; set; }
    public string Time { get; set; } = default!;
    public PlasmaType PlasmaType {  get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public PlasmaCheckInStatus Status { get; set; }
}

public enum PlasmaType
{
    DDS,
    PLM,
    FCB
}

public enum PlasmaCheckInStatus
{
    [Display(Name = "Đã lên lịch")]
    Scheduled,
    [Display(Name = "Đã Check-In")]
    CheckedIn,
    [Display(Name = "Đã hủy")]
    Canceled
}
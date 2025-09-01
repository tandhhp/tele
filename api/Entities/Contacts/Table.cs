using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Contacts;

public class Table : AuditEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
    [ForeignKey(nameof(Room))]
    public int RoomId { get; set; }
    public TableStatus Status { get; set; }
    public int SortOrder { get; set; }

    public virtual Room? Room { get; set; }
}

public enum TableStatus
{
    [Display(Name = "Sẵn sàng")]
    Available = 0,
    [Display(Name = "Đang sử dụng")]
    Occupied = 1,
    [Display(Name = "Đặt trước")]
    Reserved = 2,
    [Display(Name = "Ngưng phục vụ")]
    OutOfService = 3
}
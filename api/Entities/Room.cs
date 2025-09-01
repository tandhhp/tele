using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Waffle.Entities.Contacts;

namespace Waffle.Entities;

public class Room : AuditEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
    [ForeignKey(nameof(District))]
    public int DistrictId { get; set; }

    public virtual District? District { get; set; }
    public virtual ICollection<Table>? Tables { get; set; }
}

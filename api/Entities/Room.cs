using System.ComponentModel.DataAnnotations;
using Waffle.Entities.Contacts;

namespace Waffle.Entities;

public class Room : BaseEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;

    public virtual ICollection<Table>? Tables { get; set; }
}

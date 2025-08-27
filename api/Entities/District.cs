using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Waffle.Entities.Contacts;

namespace Waffle.Entities;

public class District : BaseEntity<int>
{
    [ForeignKey(nameof(Province))]
    public int ProvinceId { get; set; }
    [StringLength(256)]
    public string Name { get; set; } = default!;

    public Province? Province { get; set; }
    public virtual ICollection<Contact>? Contacts { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities;

public class SourceType : BaseEntity<int>
{
    [StringLength(100)]
    public string Name { get; set; } = default!;

    public virtual ICollection<Source>? Sources { get; set; }
}

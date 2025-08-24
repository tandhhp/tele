using System.ComponentModel.DataAnnotations;
using Waffle.Entities.Users;

namespace Waffle.Entities;

public class Branch : BaseEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;

    public virtual ICollection<Department>? Departments { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities.Users;

public class JobKind : BaseEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Users;

public class Team : BaseEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
    [ForeignKey(nameof(Department))]
    public int DepartmentId { get; set; }

    public Department? Department { get; set; }
}

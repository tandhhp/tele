using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities;

public class Room : BaseEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
}

﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Contacts;

public class Table : BaseEntity<int>
{
    [StringLength(256)]
    public string Name { get; set; } = default!;
    [ForeignKey(nameof(Room))]
    public int RoomId { get; set; }
    public bool Active { get; set; }
    public int SortOrder { get; set; }

    public virtual Room? Room { get; set; }
}

using Waffle.Entities.Contacts;

namespace Waffle.Core.Services.Tables.Models;

public class TableCreateArgs
{
    public int RoomId { get; set; }
    public string Name { get; set; } = default!;
    public TableStatus Status { get; set; }
    public int SortOrder { get; set; }
}

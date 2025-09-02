using Waffle.Models;

namespace Waffle.Core.Services.Tables.Models;

public class TableFilterOptions : FilterOptions
{
    public int? RoomId { get; set; }
    public int? BranchId { get; set; }
    public string? Name { get; set; }
}

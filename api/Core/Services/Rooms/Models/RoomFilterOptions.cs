using Waffle.Models;

namespace Waffle.Core.Services.Rooms.Models;

public class RoomFilterOptions : FilterOptions
{
    public string? Name { get; set; }
    public int? DistrictId { get; set; }
}

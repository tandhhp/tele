namespace Waffle.Core.Services.Rooms.Models;

public class RoomCreateArgs
{
    public int DistrictId { get; set; }
    public string Name { get; set; } = default!;
}

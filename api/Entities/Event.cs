namespace Waffle.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Time { get; set; } = default!;
    public DateTime StartDate { get; set; }

    public List<Lead>? Leads { get; set; }
}

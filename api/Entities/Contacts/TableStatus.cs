namespace Waffle.Entities.Contacts;

public class TableStatus
{
    public Guid Id { get; set; }
    public DateTime EventDate { get; set; }
    public string EventTime { get; set; } = default!;
    public bool IsAvailable { get; set; }
    public Guid TableId { get; set; }
}

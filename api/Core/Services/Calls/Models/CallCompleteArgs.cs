namespace Waffle.Core.Services.Calls.Models;

public class CallCompleteArgs
{
    public Guid ContactId { get; set; }
    public int CallStatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Note { get; set; }
    public Guid CreatedBy { get; set; }
    public string? MetaData { get; set; }
    public string? TravelHabit { get; set; }
    public string? Age { get; set; }
    public string? Job { get; set; }
    public string? ExtraStatus { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public DateTime? FollowUpTime { get; set; }
}

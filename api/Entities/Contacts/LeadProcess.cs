namespace Waffle.Entities.Contacts;

public class LeadProcess : BaseEntity
{
    public Guid LeadId { get; set; }
    public DateTime CreatedDate { get; set; }
    public LeadStatus Status { get; set; }
    public Guid UserId { get; set; }
    public string? Note { get; set; }
}

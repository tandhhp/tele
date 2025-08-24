namespace Waffle.Entities.Contacts;

public class CardHolderQueue
{
    public Guid Id { get; set; }
    public Guid CardHolderId { get; set; }
    public CardHolderQueueStatus Status { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTime? ChangedDate { get; set; }
    public Guid RequestBy { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid? LeadId { get; set; }
}

public enum CardHolderQueueStatus
{
    Pending,
    Approved,
    Rejected
}
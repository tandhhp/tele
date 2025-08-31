using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Contacts;

public class CallHistory : BaseEntity
{
    [ForeignKey(nameof(Contact))]
    public Guid ContactId { get; set; }
    [ForeignKey(nameof(CallStatus))]
    public int CallStatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Note { get; set; }
    public Guid CreatedBy { get; set; }
    public string? MetaData { get; set; }

    public virtual Contact? Contact { get; set; }
    public virtual CallStatus? CallStatus { get; set; }
}

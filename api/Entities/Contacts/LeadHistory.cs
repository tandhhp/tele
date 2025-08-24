using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities.Contacts;

public class LeadHistory
{
    [Key]
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    public Guid? SalesId { get; set; }
    public string EventTime { get; set; } = default!;
    public DateTime? EventDate { get; set; }
    public Guid? TelesaleId { get; set; }
    public string? Note { get; set; }
    public string? TableStatus { get; set; }

    public Lead? Lead { get; set; }
}

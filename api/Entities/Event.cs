using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities;

public class Event : AuditEntity<int>
{
    [StringLength(512)]
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public EventStatus Status { get; set; }
    [ForeignKey(nameof(Campaign))]
    public int? CampaignId { get; set; }

    public Campaign? Campaign { get; set; }
}

public enum EventStatus
{
    [Display(Name = "Planned")]
    Planned,
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Completed")]
    Completed,
    [Display(Name = "Cancelled")]
    Cancelled
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities;

public class Loyalty : BaseEntity
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public int Point { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public Guid? ApprovedBy { get; set; }

    public ApplicationUser? User { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Users;

public class UserPoint : BaseEntity
{
    [ForeignKey(nameof(CardHolder))]
    public Guid CardHolderId { get; set; }
    public int Point { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime DueDate { get; set; }

    public virtual ApplicationUser? CardHolder { get; set; }
}

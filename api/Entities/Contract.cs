namespace Waffle.Entities;

public class Contract : BaseEntity
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public Guid CardHolderId { get; set; }
}

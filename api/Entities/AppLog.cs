namespace Waffle.Entities;

public class AppLog : BaseEntity
{
    public Guid CatalogId { get; set; }
    public string Message { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public Guid UserId { get; set; }
}

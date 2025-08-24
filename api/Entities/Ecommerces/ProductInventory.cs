namespace Waffle.Entities;

public class ProductInventory : BaseEntity
{
    public int Quantity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public decimal Price { get; set; }
    public Guid ProductId { get; set; }
}

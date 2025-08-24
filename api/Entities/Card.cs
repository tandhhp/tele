using Microsoft.EntityFrameworkCore;

namespace Waffle.Entities;

public class Card : BaseEntity
{
    [Comment("Thời hạn hợp đồng")]
    public string? ExpiredTime { get; set; }
    [Comment("Giá trị hợp đồng")]
    public string? ContractPrice { get; set; }
    [Comment("Phí dịch vụ thẻ")]
    public string? ServicePrice { get; set; }
    [Comment("Số điểm tối đa")]
    public string? MaxLoyalty { get; set; }
    [Comment("Ưu đãi hoàn tiền")]
    public string? Refund { get; set; }
    [Comment("Điểm Top Up")]
    public string? TopUpPoint { get; set; }
    [Comment("Whynow")]
    public string? Whynow { get; set; }
    [Comment("Hạn mức ứng trước")]
    public string? Limit { get; set; }
    [Comment("Quyền lợi chính")]
    public string? Benefits { get; set; }
    public string Code { get; set; } = default!;
    public Tier Tier { get; set; }
    public string? FrontImage { get; set; }
    public string? BackImage { get; set; }
    public string? Content { get; set; }
    public int Loyalty { get; set; }
    public string? Color { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public List<ApplicationUser>? Users { get; set; }
}

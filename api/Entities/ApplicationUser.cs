using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Waffle.Entities.Users;

namespace Waffle.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [StringLength(512)]
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? Gender { get; set; }
    public int Loyalty { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? IdentityDate { get; set; }
    public string? IdentityAddress { get; set; }
    [Comment("Tiền sử sức khỏe")]
    public string? HealthHistory { get; set; }
    [Comment("Đặc điểm gia đình")]
    public string? FamilyCharacteristics { get; set; }
    [Comment("Đặc điểm tính cách")]
    public string? Personality { get; set; }
    [Comment("Mối quan tâm")]
    public string? Concerns { get; set; }
    public string? ContractCode { get; set; }
    public int Token { get; set; }
    public int NextLoyalty { get; set; }
    public DateTime? LoyaltyExpiredDate { get; set; }
    public Guid? RefId { get; set; }

    [ForeignKey(nameof(Card))]
    public Guid? CardId { get; set; }
    public Guid? SellerId { get; set; }
    public Guid? SmId { get; set; }
    public Guid? DosId { get; set; }
    public int? LoanPoint { get; set; }
    [Column(TypeName = "money")]
    public decimal Amount { get; set; }
    public bool? HasChange { get; set; }
    public Branch1 Branch { get; set; }
    public DateTime? ContractDate { get; set; }
    public int MaxLoyalty { get; set; }
    // Khi chủ thẻ được tạo từ lead thì cần gửi email cho chủ thẻ khi DOS phê duyệt
    //public bool? NeedSendEmail { get; set; }
    public Guid? TmId { get; set; }
    public UserStatus Status { get; set; }
    public Guid? TrainerId { get; set; }
    public Guid? DotId { get; set; }
    public int? TeamId { get; set; }

    [JsonIgnore]
    public Card? Card { get; set; }
    [JsonIgnore]
    public List<SubUser>? SubUsers { get; set; }

    public virtual ICollection<UserPoint>? UserPoints { get; set; }
}

public enum Branch1
{
    [Display(Name = "Miền Nam")]
    Southern,
    [Display(Name = "Miền Bắc")]
    North
}

public enum UserStatus
{
    [Display(Name = "Đang làm")]
    Working,
    [Display(Name = "Nghỉ việc")]
    Leave
}

public enum Tier
{
    Standard,
    Elite,
    Royal,
    Premier
}

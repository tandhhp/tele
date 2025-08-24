using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Waffle.Entities.Contacts;

namespace Waffle.Entities;

public class Lead
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Guid? SalesId { get; set; }
    public string EventTime { get; set; } = default!;
    public LeadStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? EventDate { get; set; }
    [Comment("CCCD")]
    public string? IdentityNumber { get; set; }
    public Branch1 Branch { get; set; }
    public bool? Gender { get; set; }
    public Guid? AccountantId { get; set; }
    public DateTime? AccountantApprovedDate { get; set; }
    public Guid? DosId { get; set; }
    public DateTime? DosApprovedDate { get; set; }
    public Guid? TmId { get; set; }
    public Guid? TelesaleId { get; set; }
    public string? Note { get; set; }
    public Guid? DotId { get; set; }
    public Guid? CreatedBy { get; set; }

    public List<SubLead>? SubLeads { get; set; }
    public List<LeadFeedback>? Feedbacks { get; set; }
}

public enum LeadStatus
{
    [Display(Name = "Chờ duyệt")]
    Pending,
    [Display(Name = "Đã duyệt")]
    Approved,
    [Display(Name = "Check-in")]
    Checkin,
    [Display(Name = "Chốt deal")]
    LeadAccept,
    [Display(Name = "Từ chối")]
    LeadReject,
    [Display(Name = "Hoàn thành")]
    Done,
    [Display(Name = "KT xác nhận")]
    AccountantApproved,
    [Display(Name = "GĐ xác nhận")]
    DosApproved,
    [Display(Name = "Mời lại")]
    ReInvite
}

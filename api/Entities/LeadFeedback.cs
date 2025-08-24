using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Waffle.Entities.Contacts;

namespace Waffle.Entities;

public class LeadFeedback
{
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    /// <summary>
    /// Tình hình tài chính
    /// </summary>
    [Comment("Tình hình tài chính")]
    public string? FinancialSituation { get; set; }
    [Comment("Mức độ quan tâm")]
    public int InterestLevel { get; set; }
    public string? RejectReason { get; set; }
    public string? TO { get; set; }
    public DateTime EventDate { get; set; }
    public string? EventTime { get; set; }
    public TimeSpan? CheckinTime { get; set; }
    public TimeSpan? CheckoutTime { get; set; }
    public string? Source { get; set; }
    public string? ContractCode { get; set; }
    [Column(TypeName = "money")]
    public decimal? ContractAmount { get; set; }
    [Column(TypeName = "money")]
    public decimal? AmountPaid { get; set; }
    public int? Age { get; set; }
    public string? JobTitle { get; set; }
    public string? Floor { get; set; }
    public string? TableStatus { get; set; }
    public Guid? TableId { get; set; }
    public string? Evidence { get; set; }
    public Guid? SellerId { get; set; }
    public string? Voucher { get; set; }
    [Comment("Xét nghiệm tại chỗ")]
    public bool? IsOnsiteTesting { get; set; }
    [StringLength(100)]
    public string? ContractCode2 { get; set; }

    public Lead? Lead {  get; set; }
    public Table? Table { get; set; }
}

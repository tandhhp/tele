using Waffle.Entities;

namespace Waffle.Models.Filters;

public class LeadFilterOptions : FilterOptions
{
    public LeadStatus? Status { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? EventTime { get; set; }
    public DateTime? EventDate { get; set; }
    public bool? InQueue { get; set; }
    public Guid? SmId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Source { get; set; }
}

public class LeadProcessFilterOptions : FilterOptions
{
    public string? Note { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? LeadName { get; set; }
    public string? UserName { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Waffle.Entities;

public class Campaign : AuditEntity<int>
{
    [StringLength(512)]
    public string Name { get; set; } = default!;
    [StringLength(256)]
    public string? Code { get; set; }
    public CampainStatus Status { get; set; }
}

public enum CampainStatus
{
    Inactive = 0,
    Active = 1,
    Completed = 2
}
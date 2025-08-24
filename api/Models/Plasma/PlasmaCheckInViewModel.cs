using System.ComponentModel.DataAnnotations.Schema;
using Waffle.Entities;
using Waffle.Entities.Plasma;

public class PlasmaUserViewModel : BaseEntity
{
    public Guid PlasmaUserId { get; set; }
    public string FullName { get; set; } = default!;
    public string? IdentityNumber { get; set; }
    public bool? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int TotalDays { get; set; }
    public Guid AssistantId { get; set; }

    [NotMapped]
    public string? SupporterName { get; set; }
    public Branch1 Branch { get; set; }
    public DateTime Date { get; set; }
    public string? Time { get; set; }
    public PlasmaType PlasmaType { get; set; }
    public PlasmaCheckInStatus Status { get; set; }
}
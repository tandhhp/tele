using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Plasma;

public class PlasmaUser : BaseEntity
{
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
    public Branch1 Branch { get; set; }
}
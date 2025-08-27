using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waffle.Entities.Contacts;

public class Contact : AuditEntity
{
    [StringLength(450)]
    public string? Name { get; set; }
    [StringLength(450)]
    public string? Email { get; set; }
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    [StringLength(1000)]
    public string? Note { get; set; }
    [StringLength(500)]
    public string? Address { get; set; }
    public ContactStatus Status { get; set; }
    [ForeignKey(nameof(Transport))]
    public int? TransportId { get; set; }
    [ForeignKey(nameof(District))]
    public int? DistrictId { get; set; }
    public bool? Gender { get; set; }
    public MarriedStatus? MarriedStatus { get; set; }
    public Guid? UserId { get; set; }

    public virtual Transport? Transport { get; set; }
    public virtual District? District { get; set; }
    public List<ContactActivity>? Activities { get; set; }
}

public enum ContactStatus
{
    New,
    Blacklisted
}

public enum MarriedStatus
{
    [Display(Name = "Độc thân")]
    Single,
    [Display(Name = "Đã kết hôn")]
    Married,
    [Display(Name = "Ly hôn")]
    Divorced,
    [Display(Name = "Góa")]
    Widowed
}

public class ContactMeta
{
    public ContactMeta()
    {
        ErrorMessage = string.Empty;
    }
    public string ErrorMessage { get; set; }
}
